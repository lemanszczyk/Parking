using System;
using System.Collections.Generic;
using System.Threading;

public interface ICar
{
    void Update(ISubject subject);
    string Rejestracja { get; set; }
    DateTime godzinaPrzyjazdu { get; set; }
}

public interface ISubject
{
    void Input(ICar observer);
    void Output(ICar observer);
    ICar ReturnCar(string rejestracja);
}
public class Subject : ISubject
{
    public int State { get; set; } = -0;
    private List<ICar> _observers = new List<ICar>();
    public void Input(ICar observer)
    {
        if (_observers.Count > 100)
        {
            Console.WriteLine("Wszystkie miejsca zajęte");
            return;
        }
        this.State = 1;
        observer.Update(this);
        this._observers.Add(observer);
        Console.WriteLine("Dodano samochód do parkingu");
        Console.WriteLine("Pozostało ilość miejsc:  " + (100 - _observers.Count));
    }

    public void Output(ICar observer)
    {
        if (!_observers.Any(x => x.Equals(observer)))
        {
            Console.WriteLine("Błąd, brak samochodu o takim numerze");
            return;
        }
        this.State = 2;
        this._observers.Remove(observer);
        Console.WriteLine("Pozostało ilość miejsc " + +(100 - _observers.Count));
        observer.Update(this);
    }

    public ICar? ReturnCar(string rejestracja)
    {
        var car = _observers.FirstOrDefault(x => x.Rejestracja == rejestracja);
        if (car != null)
        {
            return car;
        }
        Console.WriteLine("Błąd, brak samochodu o takim numerze");
        return null;
    }
}
class Car : ICar
{
    public string Rejestracja { get; set; }
    public DateTime godzinaPrzyjazdu { get; set; }

    public void Update(ISubject subject)
    {
        if ((subject as Subject).State == 1)
        {
            godzinaPrzyjazdu = DateTime.Now;
        }
        else if ((subject as Subject).State == 2)
        {
            var godzinaOdjazdu = DateTime.Now;
            var time = godzinaOdjazdu.Subtract(godzinaPrzyjazdu);
            Console.WriteLine("Do drukarki wysłano paragon o ilości " + (time.TotalSeconds * 0.1 * 0.01) + " PLN");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var parking = new Subject();

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
                    var car = new Car();
                    car.Rejestracja = rejestracja;
                    parking.Input(car);
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
                    var car = parking.ReturnCar(rejestracja);
                    if (car != null)
                    {
                        parking.Output(car);
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