using System;

namespace DataAccess
{
    public static class clsDataAccessSettings
    {
        // يجب أن يكون public و static ليراه الآخرون
        public static string ConnectionString = @"Server=.\SQLEXPRESS;Database=DVLD;Integrated Security = true;";
    }
}