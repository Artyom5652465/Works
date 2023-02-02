using System;
using System.Collections.Generic;

namespace GameClub
{
    class Program
    {
        static void Main(string[] args)
        {
            ComputerClub club = new ComputerClub(10);
            club.Work();
        }
    }

    class ComputerClub
    {
        private int _money = 0;
        private List<Computer> _computers = new List<Computer>();
        private Queue<Client> _clients = new Queue<Client>();

        public ComputerClub(int computerCount)
        {
            Random random = new Random();
            for (int i = 0; i < computerCount; i++)
            {
                _computers.Add(new Computer(random.Next(5, 15)));
            }

            CreateNewClients(25, random);
        }

        public void CreateNewClients(int count, Random random)
        {
            for (int i = 0; i < count; i++)
            {
                _clients.Enqueue(new Client(random.Next(100, 251), random));
            }
        }

        public void Work()
        {
            while(_clients.Count>0)
            {
                Client newClient = _clients.Dequeue();
                Console.WriteLine($"Баланс компьютерного клуба: {_money} руб. Ждём нового клиента.");
                Console.WriteLine($"У вас новый клиент, и он хочет купить {newClient.DesiredMinutes} минут");
                ShowAllComputerState();

                Console.WriteLine("Вы предлагаете компьютер под номером: ");
                string userInput = Console.ReadLine();
                
                if(int.TryParse(userInput, out int computerNumber))
                {
                    computerNumber -= 1;

                    if(computerNumber >= 0 && computerNumber < _computers.Count)
                    {
                        if(_computers[computerNumber].IsTaken)
                        {
                            Console.WriteLine("Вы пытаетесь посадить за стол, который уже занят. Клиент разозлился и ушёл.");
                        }
                        else
                        {
                            if(newClient.CheckSolvency(_computers[computerNumber]))
                            {
                                Console.WriteLine("Клиент, персчитав деньги, оплатил и сел за компьютер " + (computerNumber + 1));
                                _money = newClient.Pay();
                                _computers[computerNumber].BecomeTaken(newClient);
                            }
                            else
                            {
                                Console.WriteLine("У клиента не хватило и денег и он ушёл.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Вы не знаете, за какой компьютер посадить клиента. Он разозлился и ушёл.");
                    }
                }
                else
                {
                    CreateNewClients(1, new Random());
                    Console.WriteLine("Неверный ввод! Повторите снова.");
                }

                Console.WriteLine("Чтобы перейти к следущему клиенту, нажмите любую клавишу.");
                Console.ReadKey();
                Console.Clear();
                SpendOnMinute();
            }
        }
        private void SpendOnMinute()
        {
            foreach(var computer in _computers)
            {
                computer.SpendMinute();
            }
        }

        public void ShowAllComputerState()
        {
            Console.WriteLine("\nСписок всех компьютеров:");
            for (int i = 0; i < _computers.Count; i++)
            {
                Console.Write(i + 1 + " - ");
                _computers[i].ShowState();
            }
        }
    }

    class Computer
    {
        private int _minutesRemainig;
        private Client _client;
        public bool IsTaken
        {
            get
            {
                return _minutesRemainig > 0;
            }
        }
        public int PricePerMinute { get; private set; }

        public Computer(int pricePerMinute)
        {
            PricePerMinute = pricePerMinute;
        }

        public void BecomeTaken(Client client)
        {
            _client = client;
            _minutesRemainig = client.DesiredMinutes;
        }

        public void BecomeEmpty()
        {
            _client = null;
        }

        public void SpendMinute()
        {
            _minutesRemainig--;
        }

        public void ShowState()
        {
            if(IsTaken)
                Console.WriteLine($"Компьютер занят. Осталось минут: {_minutesRemainig}");
            else
                Console.WriteLine($"Компьютер свободен. Цена за минуту: {PricePerMinute}");
        }
    }

    class Client
    {
        private int _money;
        private int _moneyToPay;
        public int DesiredMinutes { get; private set; }
        public Client(int money, Random rand)
        {
            _money = money;
            DesiredMinutes = rand.Next(10, 30);
        }

        public bool CheckSolvency(Computer computer)
        {
            _moneyToPay = DesiredMinutes * computer.PricePerMinute;
            if(_moneyToPay<=_money)
            {
                return true;
            }
            else
            {
                _moneyToPay = 0;
                return false;
            }
        }

        public int Pay()
        {
            _money -= _moneyToPay;
            return _moneyToPay;
        }
    }
}
