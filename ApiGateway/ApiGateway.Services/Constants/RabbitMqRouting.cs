using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Services.Constants
{
    public static class RabbitMqRouting
    {
        public const string ExchangeEmail = "email.exchange";
        public const string EmailConfirm = "email.confirm";
        public const string EmailForgot = "email.forgot";
    }
}