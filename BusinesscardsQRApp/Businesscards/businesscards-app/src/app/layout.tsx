import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import Link from "next/link";
import "./globals.css";
import Footer from "@/components/Footer";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Digital Business Card App",
  description: "Create and Manage your digital business cards with QR Codes",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className={`${geistSans.variable} ${geistMono.variable} antialiased`}>
        <div className="flex flex-col min-h-screen">
          {/* Header */}          
        <header className="bg-gray-800 text-white p-4">
          <nav className="max-w-4xl mx-auto flex space-x-6">
            <Link href="/" className="hover:underline">Home</Link>
            <Link href="/business-cards" className="hover:underline">Create Card</Link>
            <Link href="/manage-cards" className="hover:underline">Manage Cards</Link>
          </nav>
        </header>
        {/* Main Content */}
        <main className="flex-grow max-w-4xl mx-auto p-6">
        {children}
        </main>
        {/* Sticky Footer */}
        <Footer />
        </div>
      </body>
    </html>
  );
}
