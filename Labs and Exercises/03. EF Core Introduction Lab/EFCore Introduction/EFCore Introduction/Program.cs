using System;

namespace EFCore_Introduction
{
    class Program
    {
        static void Main(string[] args)
        {
            //dotnet ef dbcontext scaffold "Server=.\SQLEXPRESS; Database=Softuni; Integrated Security=true" Microsoft.EntityFrameworkCore.SqlServer -o DBStuff  create in folder
            //dotnet ef dbcontext scaffold "Server=.\SQLEXPRESS; Database=Softuni; Integrated Security=true" Microsoft.EntityFrameworkCore.SqlServer -o DBStuff -f recreate with changes
            //dotnet ef dbcontext scaffold "Server=.\SQLEXPRESS; Database=Softuni; Integrated Security=true" Microsoft.EntityFrameworkCore.SqlServer -o DBStuff -f -d use attributes 
        }
    }
}
