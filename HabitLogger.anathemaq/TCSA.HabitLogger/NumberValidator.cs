namespace TCSA.HabitLogger;

public class NumberValidator
{
    public bool ValidateNumber(string input , out int result)
    {
        if(int.TryParse(input, out result) && result > 0)
        {
            return true;
        }
        return  false;
    }
}