using System;

namespace Federation.Core
{
    public interface ISecurityService
    {
        int Version { get; }

        Guid CalcPasswordHash(string password, Guid salt);

        void GeneratePasswordHash(string password, out Guid hash, out Guid salt);

        User EmailUserValidation (string email, string password);
        User LoginUserValidation (string login, string password);
    }
}