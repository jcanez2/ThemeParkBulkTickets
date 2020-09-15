using System;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace Project2
{
    public delegate void priceCutEvent(int price); // Define a delegate

    public class ThemePark
    {
        static Random priceChange = new Random(); // generate random numbers
        public static event priceCutEvent priceCut; // Link event to delegate
        public static int TicketPrice = 10;

        public int getPrice()
        {
            return TicketPrice;
        }

        public static void ChangePrice(int price)
        {
            if (price < TicketPrice)
            {
                if (priceCut != null) // there is a subscriber
                {
                    priceCut(price);
                }
            }

            TicketPrice = price;
        }

        public void ThemeParkFunc()
        {
            for (int i = 0; i < 50; i++)
            {
                Thread.Sleep(500);
                // Take the order from the queue of the orders;
                // Decide the price based on the orders
                int newTicketPrice = priceChange.Next(5, 10);
                Console.WriteLine("New Price is {0}", newTicketPrice);
                ThemePark.ChangePrice(newTicketPrice);
            }
        }

    }

    public class TicketAgency
    {
        public void TicketAgencyFunc()
        {
            ThemePark park = new ThemePark();
            for (int i = 0; i < 10; i++)
            {
                Thread.Sleep(1000);
                int price = park.getPrice();
                Console.WriteLine("Theme Park {0} has everyday low price of ${1} each ticket", Thread.CurrentThread.Name, price);

            }
        }

        public void TicketOnSale(int price) // handles event
        {
            // order tickets from Theme Park - send order to queue
            //Console.WriteLine("Console.WriteLine("Thread {0}: tickets are on sale as low as ${1} each.", Thread.CurrentThread.Name, price);
            Console.WriteLine("Thread {0}: Tickets are on sale at the price of ${1}", Thread.CurrentThread.Name, price);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
           ThemePark disneyLand = new ThemePark();
           Thread processTickets = new Thread(new ThreadStart(disneyLand.ThemeParkFunc));
           processTickets.Start(); // start one consumer thread
           processTickets.Name = "processTickets";

           TicketAgency touristCenter = new TicketAgency();

           ThemePark.priceCut += new priceCutEvent(touristCenter.TicketOnSale);

           Thread[] ticketAgencies = new Thread[3];
           for (int i = 0; i < 3; i++)
           {
               // Start n ticketAgency threads
               ticketAgencies[i] = new Thread(new ThreadStart(touristCenter.TicketAgencyFunc));
               ticketAgencies[i].Name = (i + 1).ToString();
               ticketAgencies[i].Start();
           }
        }
    }
}
