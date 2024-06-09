In "Utils/RuleUtils.cs" - From line 294: "Feature 'switch expression' is not available. Please use language version 8.0 or greater.".

Possible solution: change method into the equivalent following one. For now the old code is commented and the new one
replaced it.


    public static string FirstCharToUpper(string input)
    {
        if (input == null)
        {
        throw new ArgumentNullException(nameof(input));
        }
        switch (input)
        {
            case "":
                throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
            default:
                return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }

Some methods has been moved into a commented section: methods that have problems or refer to UI classes and methods have 
been moved to a commented section, waiting to be removed from the library code. Other methods that have some problems or
I am not sure about what to do have been mover in another specific commented section.