﻿namespace BalanceKeeper.Repositories
{
    internal class ResetPasswordWithCodeViewModel
    {
     

        public string Email { get; set; }
        public string ResetCode { get; set; }
        public string Password { get; set; }
    }
}