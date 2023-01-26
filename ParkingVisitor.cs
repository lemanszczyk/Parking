using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Timers;

public interface IObserver
{
    void Update(ISubject subject);
}

public interface ISubject
{
    void Attach(IObserver observer);
    void Detach(IObserver observer);
    void Notify();
}
public class Subject : ISubject
{
    public int State { get; set; } = -0;
    private List<IObserver> _observers = new List<IObserver>();
    public List<Car>? Cars = new List<Car>();
    public double LeftToPay { get; set; }
    public void Attach(IObserver observer)
    {
        Console.WriteLine("Dodano nowego observera");
        _observers.Add(observer);
    }
    public void Detach(IObserver observer)
    {
        Console.WriteLine("Usunieto observera");
        _observers.Remove(observer);
    }
    public void Notify()
    {
        foreach (var observer in _observers)
        {
            observer.Update(this);
        }
    }
    public void CarArrive(Car car)
    {
        this.State = 1;
        if (Cars.Count == 100)
        {
            Console.WriteLine("Brak miejsc na parkingu");
            return;
        }
        else if (Cars.Any(x => x.Rejestracja == car.Rejestracja))
        {
            Console.WriteLine("Istnieje już samochód o takim numerze rejestracyjnym");
            return;
        }
        Cars.Add(car);
        this.Notify();
    }
    public void CarLeft(Car car)
    {
        Cars.Remove(car);
        this.State = 2;
        var godzinaOdjazdu = DateTime.Now;
        var time = godzinaOdjazdu.Subtract(car.godzinaPrzyjazdu);
        LeftToPay = time.TotalSeconds * 0.1 * 0.01;
        this.Notify();
    }
}
class Ekran : IObserver
{
    public void Update(ISubject subject)
    {
        Console.WriteLine("Pozostało ilość miejsc " + (100 - (subject as Subject).Cars.Count));
    }
}
class Drukarka : IObserver
{
    public void Update(ISubject subject)
    {
        if ((subject as Subject).State == 2)
        {
            Console.WriteLine("Do drukarki wysłano paragon o ilości " + (subject as Subject).LeftToPay + " PLN");
        }
    }

}
public class Car
{
    public string Rejestracja;
    public DateTime godzinaPrzyjazdu;
    public Car(string rejestracja, DateTime godzinaPrzyjazdu)
    {
        Rejestracja = rejestracja;
        this.godzinaPrzyjazdu = godzinaPrzyjazdu;
    }
}

class Program
{
    static void Main(string[] args)
    {
        var parking = new Subject();
        var drukarkaObs = new Drukarka();
        var ekranObs = new Ekran();
        parking.Attach(drukarkaObs);
        parking.Attach(ekranObs);

        while (true)
        {
            Console.WriteLine("Podaj rodzaj operacji(I/O): ");
            var operation = Console.ReadLine();
            if (operation == "I")
            {
                Console.WriteLine("Podaj numer rejestracyjny samochodu: ");
                var rejestracja = Console.ReadLine();
                if (rejestracja != null)
                {
                    var car = new Car(rejestracja, DateTime.Now);
                    parking.CarArrive(car);
                }
                else
                {
                    Console.WriteLine("Źle podana rejestracja");
                }
            }
            else if (operation == "O")
            {
                Console.WriteLine("Podaj numer rejestracyjny samochodu: ");
                var rejestracja = Console.ReadLine();
                if (rejestracja != null)
                {
                    var car = parking.Cars.FirstOrDefault(x => x.Rejestracja == rejestracja);
                    if (car != null)
                    {
                        parking.CarLeft(car);
                    }
                    else
                    {
                        Console.WriteLine("Błąd, brak samochodu o takim numerze");
                    }
                }
                else
                {
                    Console.WriteLine("Źle podana rejestracja");
                }
            }
            else
            {
                Console.WriteLine("Błąd wyboru operacji");
            }
        }
    }
}