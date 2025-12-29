// Configuration object
const config = {
  apiRootUrl: "https://localhost:5001",
};

class StockWidget {
  constructor(ticker, connection) {
    this.ticker = ticker;
    this.connection = connection;
    this.element = this.createWidget();
    this.priceElement = this.element.querySelector(".stock-price");
    this.changeElement = this.element.querySelector(".stock-change");
    this.chartElement = this.element.querySelector(".stock-chart");
    this.lastPrice = null;
    this.priceHistory = [];
    this.minPrice = Infinity;
    this.maxPrice = -Infinity;
    this.chart = this.createChart();
    this.fetchPrice();
  }

  createWidget() {
    const widget = document.createElement("div");
    widget.className =
      "bg-white dark:bg-gray-800 rounded-xl shadow-md p-6 relative transition-colors duration-200";
    widget.innerHTML = `
      <button class="remove-stock absolute top-3 right-3 text-gray-400 hover:text-red-500 dark:text-gray-500 dark:hover:text-red-400">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" viewBox="0 0 20 20" fill="currentColor">
          <path fill-rule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clip-rule="evenodd" />
        </svg>
      </button>
      <h2 class="text-2xl font-bold mb-4 text-gray-900 dark:text-white">${this.ticker}</h2>
      <div class="flex items-center justify-between">
        <div class="flex-grow">
          <p class="stock-price text-4xl font-bold mb-2 text-gray-900 dark:text-white">Loading...</p>
          <p class="stock-change text-xl font-semibold"></p>
        </div>
        <div class="w-40 h-24">
          <canvas class="stock-chart"></canvas>
        </div>
      </div>
    `;
    widget
      .querySelector(".remove-stock")
      .addEventListener("click", () => this.remove());
    return widget;
  }

  createChart() {
    return new Chart(this.chartElement, {
      type: "line",
      data: {
        labels: [],
        datasets: [
          {
            data: [],
            borderColor: "rgb(75, 192, 192)",
            tension: 0.1,
            pointRadius: 0,
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false },
          tooltip: { enabled: false },
        },
        scales: {
          x: { display: false },
          y: {
            display: true,
            position: "right",
            grid: { display: false },
            ticks: {
              color: "rgb(156, 163, 175)", // gray-400 in Tailwind
              font: { size: 10 },
              callback: function (value, index, values) {
                // Only show the top and bottom values
                if (index === 0 || index === values.length - 1) {
                  return "$" + value.toFixed(2);
                }
                return "";
              },
              count: 2, // This ensures only 2 ticks are shown
            },
          },
        },
        animation: false,
      },
    });
  }

  async fetchPrice() {
    try {
      const response = await fetch(
        `${config.apiRootUrl}/api/stocks/${this.ticker}`
      );
      const data = await response.json();

      if (data.price) {
        this.updatePrice(data.price);
        // Join the SignalR group for this stock
        await this.connection.invoke("JoinStockGroup", this.ticker);
        console.log(`Joined SignalR group for ${this.ticker}`);
      } else {
        this.priceElement.textContent = "Unable to fetch price";
      }
    } catch (error) {
      console.error("Error fetching stock price:", error);
      this.priceElement.textContent = "Error fetching price";
    }
  }

  updatePrice(newPrice) {
    const formattedPrice = `$${newPrice.toFixed(2)}`;
    this.priceElement.textContent = formattedPrice;

    if (this.lastPrice !== null) {
      const change = newPrice - this.lastPrice;
      const changePercent = (change / this.lastPrice) * 100;
      const changeText = `${change >= 0 ? "+" : ""}${change.toFixed(
        2
      )} (${changePercent.toFixed(2)}%)`;
      this.changeElement.textContent = changeText;
      this.changeElement.className = `stock-change text-xl font-semibold ${
        change >= 0 ? "text-green-600" : "text-red-600"
      }`;
    }

    this.updateChart(newPrice);
    this.lastPrice = newPrice;
  }

  updateChart(price) {
    this.priceHistory.push(price);

    // Keep only the last 30 price points
    if (this.priceHistory.length > 30) {
      this.priceHistory.shift();
    }

    // Update min and max prices
    this.minPrice = Math.min(...this.priceHistory);
    this.maxPrice = Math.max(...this.priceHistory);

    this.chart.data.labels = this.priceHistory.map((_, index) => index + 1);
    this.chart.data.datasets[0].data = this.priceHistory;

    // Update y-axis min and max with a little padding
    const range = this.maxPrice - this.minPrice;
    const padding = range * 0.1; // 10% padding
    this.chart.options.scales.y.min = this.minPrice - padding;
    this.chart.options.scales.y.max = this.maxPrice + padding;

    this.chart.update();
  }

  async remove() {
    // Leave the SignalR group for this stock
    try {
      await this.connection.invoke("LeaveStockGroup", this.ticker);
      console.log(`Left SignalR group for ${this.ticker}`);
    } catch (error) {
      console.error(`Error leaving SignalR group for ${this.ticker}:`, error);
    }

    // Destroy the Chart instance
    if (this.chart) {
      this.chart.destroy();
    }
    // Remove the element from the DOM
    this.element.remove();
    // Signal that this widget has been removed
    if (typeof this.onRemove === "function") {
      this.onRemove(this.ticker);
    }
  }
}

document.addEventListener("DOMContentLoaded", () => {
  const tickerInput = document.getElementById("tickerInput");
  const addStockButton = document.getElementById("addStockButton");
  const stockContainer = document.getElementById("stockContainer");
  const darkModeToggle = document.getElementById("darkModeToggle");
  const stockWidgets = new Map();

  // Dark mode toggle functionality
  darkModeToggle.addEventListener("click", () => {
    document.documentElement.classList.toggle("dark");
    localStorage.setItem(
      "darkMode",
      document.documentElement.classList.contains("dark")
    );
  });

  // Check for saved dark mode preference
  if (localStorage.getItem("darkMode") === "true") {
    document.documentElement.classList.add("dark");
  } else {
    document.documentElement.classList.remove("dark");
  }

  // Set up SignalR connection
  const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${config.apiRootUrl}/stocks-feed`)
    .configureLogging(signalR.LogLevel.Information)
    .build();

  async function start() {
    try {
      await connection.start();
      console.log("SignalR Connected.");
      // Load predefined tickers after connection is established
      loadPredefinedTickers();
    } catch (err) {
      console.log(err);
      setTimeout(start, 5000);
    }
  }

  connection.onclose(async () => {
    await start();
  });

  // Start the connection.
  start();

  // Handle incoming stock updates
  connection.on("ReceiveStockPriceUpdate", (stockUpdate) => {
    if (stockWidgets.has(stockUpdate.ticker)) {
      stockWidgets.get(stockUpdate.ticker).updatePrice(stockUpdate.price);
    }
  });

  async function addStock(ticker) {
    if (!stockWidgets.has(ticker)) {
      const stockWidget = new StockWidget(ticker, connection);
      stockWidget.onRemove = (removedTicker) => {
        stockWidgets.delete(removedTicker);
        console.log(`Removed ${removedTicker} from stockWidgets`);
        console.log("Current stocks:", Array.from(stockWidgets.keys()));
      };
      stockContainer.appendChild(stockWidget.element);
      stockWidgets.set(ticker, stockWidget);
      console.log(`Added ${ticker} to stockWidgets`);
      console.log("Current stocks:", Array.from(stockWidgets.keys()));
    } else {
      alert("This stock is already in your dashboard");
    }
  }

  addStockButton.addEventListener("click", () => {
    const ticker = tickerInput.value.toUpperCase();
    if (!ticker) {
      alert("Please enter a ticker symbol");
      return;
    }

    addStock(ticker);
    tickerInput.value = "";
  });

  // Function to load predefined tickers
  function loadPredefinedTickers() {
    const tickers = ["AMZN", "MSFT", "META", "NVDA", "TSLA", "BABA", "PYPL"];
    tickers.forEach((ticker) => addStock(ticker));
  }
});
