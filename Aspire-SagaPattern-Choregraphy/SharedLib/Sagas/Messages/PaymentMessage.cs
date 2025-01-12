using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Sagas.Messages;

public class PaymentMessage
{
    public string CardNumber { get; set; }
    public string Expiration { get; set; }
    public string CardHolderName { get; set; }
    public string CVV { get; set; } 
}
