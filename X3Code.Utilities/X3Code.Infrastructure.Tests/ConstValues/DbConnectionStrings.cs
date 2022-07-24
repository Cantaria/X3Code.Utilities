﻿namespace X3Code.Infrastructure.Tests.ConstValues
{
    public static class DbConnectionStrings
    {
        private const string LocalClemensConnectionString = "Server=virtual.local;Database={0};User Id=SA;Password=Password1;TrustServerCertificate=True";   //VM Server
        private const string LocalMichaelConnectionString = "Server=192.168.188.20;Database={0};User Id=sa;Password=yourStrong(!)Password;";   //VM Server

        public static string GetConnectionString()
        {
            return string.Format(LocalClemensConnectionString, "UnitTest");
        }
    }
}