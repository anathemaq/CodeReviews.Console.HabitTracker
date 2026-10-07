namespace TCSA.HabitLogger.Tests;

public class NumberValidatorTests
{
    NumberValidator numberValidator = new ();
    
    [Theory]
    [InlineData("15", true, 15)]
    [InlineData("-15", false, -15)]
    [InlineData("abc", false, 0)]
    [InlineData("0", false, 0)]
    [InlineData("999999999999999999999999", false, 0)]
    
    public void ValidateNumber_WithDifferentInputs_ReturnsExpectedResults(string input, bool expectedValid, int expectedNumber)
    {
        //Act
        bool isValid = numberValidator.ValidateNumber(input, out int number);
        //Assert
        Assert.Equal(expectedValid, isValid);
        Assert.Equal(expectedNumber, number);
    }
}