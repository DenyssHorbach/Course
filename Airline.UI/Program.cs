using System.Text;
using Airline.Core.Exceptions;
using Airline.Core.Models;
using Airline.Core.Services;

namespace Airline.UI;

internal class Program
{
    private static readonly AirlineService _service = new("data");

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        try
        {
            _service.LoadAll();
            if (_service.GetAirports().Count == 0 && _service.GetFlights().Count == 0)
            {
                _service.SeedSampleData();
                Console.WriteLine("База порожня — завантажено стартові тестові дані.");
            }
            else
            {
                Console.WriteLine("Дані успішно завантажено з JSON-файлів.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка завантаження даних: {ex.Message}");
        }

        bool running = true;
        while (running)
        {
            PrintMenu();
            int choice = ConsoleHelper.ReadInt("Оберіть дію: ", 0, 8);
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case 1: ShowFlights(); break;
                    case 2: SearchFlights(); break;
                    case 3: BookTicket(); break;
                    case 4: ShowTickets(); break;
                    case 5: AddAirport(); break;
                    case 6: AddFlight(); break;
                    case 7: AddPassenger(); break;
                    case 8: CancelTicket(); break;
                    case 0:
                        _service.SaveAll();
                        Console.WriteLine("Дані збережено у папку data/. Роботу завершено.");
                        running = false;
                        break;
                }
            }
            catch (OverbookingException ex)
            {
                Console.WriteLine($"[ПОМИЛКА БРОНЮВАННЯ] {ex.Message}");
            }
            catch (BaggageWeightLimitExceededException ex)
            {
                Console.WriteLine($"[ПОМИЛКА БАГАЖУ] {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[ПОМИЛКА ВАЛІДАЦІЇ] {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[КРИТИЧНА ПОМИЛКА] {ex.Message}");
            }
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine("\n ГОЛОВНЕ МЕНЮ ");
        Console.WriteLine("1. Переглянути рейси");
        Console.WriteLine("2. Пошук рейсів за напрямком");
        Console.WriteLine("3. Купити квиток");
        Console.WriteLine("4. Переглянути продані квитки");
        Console.WriteLine("5. Додати аеропорт");
        Console.WriteLine("6. Додати рейс");
        Console.WriteLine("7. Додати пасажира");
        Console.WriteLine("8. Скасувати (видалити) квиток");
        Console.WriteLine("0. Зберегти та вийти");
    }

    private static void ShowFlights()
    {
        int sortMode = ConsoleHelper.ReadInt("Сортування (1 — за датою, 2 — за ціною): ", 1, 2);
        var flights = _service.GetFlights(sortByPrice: sortMode == 2);
        PrintFlightsList(flights);
    }

    private static void SearchFlights()
    {
        string query = ConsoleHelper.ReadNonEmptyString("Введіть місто, країну або код аеропорту призначення (наприклад, London або LHR): ");
        var flights = _service.SearchFlightsByDestination(query);
        PrintFlightsList(flights);
    }

    private static void PrintFlightsList(List<Flight> flights)
    {
        if (flights.Count == 0)
        {
            Console.WriteLine("Рейсів не знайдено.");
            return;
        }

        for (int i = 0; i < flights.Count; i++)
        {
            Flight f = flights[i];
            string from = _service.GetAirportById(f.DepartureAirportId)?.Code ?? "???";
            string to = _service.GetAirportById(f.ArrivalAirportId)?.City ?? "???";
            string toCode = _service.GetAirportById(f.ArrivalAirportId)?.Code ?? "???";
            int booked = _service.GetBookedSeatsCount(f.Id);

            Console.WriteLine(
                $"{i + 1}. [{f.FlightNumber}] {from} -> {toCode} ({to}) | " +
                $"{f.DepartureTime:dd.MM.yyyy HH:mm} | Базова ціна: {f.BasePrice} грн | " +
                $"Місця: {booked}/{f.MaxPassengers} | Багаж: до {f.FreeBaggageWeightKg} кг (макс. {f.MaxBaggageWeightKg} кг)");
        }
    }

    private static void BookTicket()
    {
        var flights = _service.GetFlights();
        var passengers = _service.GetPassengers();

        if (flights.Count == 0 || passengers.Count == 0)
        {
            Console.WriteLine("Спочатку додайте хоча б один рейс та одного пасажира.");
            return;
        }

        Console.WriteLine("Оберіть рейс:");
        PrintFlightsList(flights);
        int flightIndex = ConsoleHelper.ReadInt("Номер рейсу зі списку: ", 1, flights.Count) - 1;

        Console.WriteLine("\nОберіть пасажира:");
        for (int i = 0; i < passengers.Count; i++)
            Console.WriteLine($"{i + 1}. {passengers[i]}");
        int passengerIndex = ConsoleHelper.ReadInt("Номер пасажира зі списку: ", 1, passengers.Count) - 1;

        Console.WriteLine("\nКлас обслуговування: 1 — Economy (x1.0), 2 — Business (x1.8), 3 — First (x2.5)");
        var serviceClass = (ServiceClass)ConsoleHelper.ReadInt("Оберіть клас: ", 1, 3);

        double baggageWeight = ConsoleHelper.ReadDouble("Вага багажу (кг): ", 0, 200);

        Ticket ticket = _service.BookTicket(
            flights[flightIndex].Id,
            passengers[passengerIndex].Id,
            serviceClass,
            baggageWeight);

        Console.WriteLine($"\nКвиток успішно оформлено! ID: {ticket.Id}");
        Console.WriteLine($"Клас: {ticket.ServiceClass} | Доплата за багаж: {ticket.ExtraBaggageFee} грн | Разом: {ticket.TotalPrice} грн");
    }

    private static void ShowTickets()
    {
        var tickets = _service.GetTickets();
        if (tickets.Count == 0)
        {
            Console.WriteLine("Проданих квитків ще немає.");
            return;
        }

        for (int i = 0; i < tickets.Count; i++)
        {
            Ticket t = tickets[i];
            string flightNum = _service.GetFlightById(t.FlightId)?.FlightNumber ?? "Невідомий";
            string passengerName = _service.GetPassengerById(t.PassengerId)?.FullName ?? "Невідомий";

            Console.WriteLine(
                $"{i + 1}. Рейс: {flightNum} | Пасажир: {passengerName} | Клас: {t.ServiceClass} | " +
                $"Багаж: {t.BaggageWeightKg} кг (доплата: {t.ExtraBaggageFee} грн) | Всього: {t.TotalPrice} грн");
        }
    }

    private static void AddAirport()
    {
        string code = ConsoleHelper.ReadNonEmptyString("Код IATA (3 літери, напр. KBP): ");
        string name = ConsoleHelper.ReadNonEmptyString("Назва аеропорту: ");
        string city = ConsoleHelper.ReadNonEmptyString("Місто: ");
        string country = ConsoleHelper.ReadNonEmptyString("Країна: ");

        _service.AddAirport(new Airport(code, name, city, country));
        Console.WriteLine("Аеропорт успішно додано.");
    }

    private static void AddFlight()
    {
        var airports = _service.GetAirports();
        if (airports.Count < 2)
        {
            Console.WriteLine("Потрібно щонайменше 2 аеропорти.");
            return;
        }

        for (int i = 0; i < airports.Count; i++)
            Console.WriteLine($"{i + 1}. {airports[i]}");

        string number = ConsoleHelper.ReadNonEmptyString("Номер рейсу (напр. PS-101): ");
        int depIndex = ConsoleHelper.ReadInt("Аеропорт вильоту: ", 1, airports.Count) - 1;
        int arrIndex = ConsoleHelper.ReadInt("Аеропорт прильоту: ", 1, airports.Count) - 1;
        DateTime depTime = ConsoleHelper.ReadDateTime("Дата та час вильоту (дд.мм.рррр гг:хх): ");
        decimal price = ConsoleHelper.ReadDecimal("Базова ціна (грн): ", 1, 1_000_000);
        int maxPassengers = ConsoleHelper.ReadInt("Кількість місць: ", 1, 1000);
        double freeBaggage = ConsoleHelper.ReadDouble("Безкоштовний багаж (кг): ", 0, 50);
        double maxBaggage = ConsoleHelper.ReadDouble("Макс. вага багажу (кг): ", 1, 100);
        decimal extraPerKg = ConsoleHelper.ReadDecimal("Доплата за 1 кг перевищення (грн): ", 0, 10_000);

        var flight = new Flight(
            number, airports[depIndex].Id, airports[arrIndex].Id,
            depTime, price, maxPassengers, freeBaggage, maxBaggage, extraPerKg);

        _service.AddFlight(flight);
        Console.WriteLine("Рейс успішно додано.");
    }

    private static void AddPassenger()
    {
        string fullName = ConsoleHelper.ReadNonEmptyString("ПІБ пасажира: ");
        string passport = ConsoleHelper.ReadNonEmptyString("Номер паспорта: ");
        string phone = ConsoleHelper.ReadNonEmptyString("Телефон: ");

        _service.AddPassenger(new Passenger(fullName, passport, phone));
        Console.WriteLine("Пасажира успішно додано.");
    }

    private static void CancelTicket()
    {
        var tickets = _service.GetTickets();
        if (tickets.Count == 0)
        {
            Console.WriteLine("Немає квитків для скасування.");
            return;
        }

        ShowTickets();
        int index = ConsoleHelper.ReadInt("Введіть номер квитка для скасування: ", 1, tickets.Count) - 1;
        if (_service.CancelTicket(tickets[index].Id))
            Console.WriteLine("Квиток скасовано.");
    }
}