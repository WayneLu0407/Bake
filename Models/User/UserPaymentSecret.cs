using System;
using System.Collections.Generic;

namespace Bake.Models.User;

public partial class UserPaymentSecret
{
    public int UserId { get; set; }

    public byte[] EncryptedBankAcc { get; set; } = null!;

    public virtual AccountAuth User { get; set; } = null!;
}
