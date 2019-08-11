
//
/// <summary>
/// ***************************************************************************
/// Copyright (c) 2005, Industrial Logic, Inc., All Rights Reserved.
///
/// This code is the exclusive property of Industrial Logic, Inc. It may ONLY be
/// used by students during Industrial Logic's workshops or by individuals
/// who are being coached by Industrial Logic on a project.
///
/// This code may NOT be copied or used for any other purpose without the prior
/// written consent of Industrial Logic, Inc.
/// ****************************************************************************
/// </summary>

namespace IndustrialLogic.Strategy
{
    public class CapitalStrategyAdvisedLine : CapitalStrategy
    {
        protected override double RiskAmount(Loan loan)
        {
            return loan.Commitment * loan.UnusedPercentage;
        }
    }
}
