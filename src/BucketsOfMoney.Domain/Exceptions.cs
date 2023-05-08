using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BucketsOfMoney.Domain
{
    public class IngressStrategyMinimumAmountViolation : Exception { }
    public class IngressStrategyMaximumAmountViolation : Exception { }
    public class AccountMinimumAmountViolation : Exception {}
}
