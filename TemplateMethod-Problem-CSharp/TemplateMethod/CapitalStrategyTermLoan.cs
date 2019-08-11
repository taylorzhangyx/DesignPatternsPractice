
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
    public class CapitalStrategyTermLoan : CapitalStrategy
    {

        protected override double RiskAmount(Loan loan)
        {
            return loan.Commitment;
        }

        public override double Duration(Loan loan)
        {
            return WeightedAverageDuration(loan);
        }

        private double WeightedAverageDuration(Loan loan)
        {
            double duration = 0.0;
            double weightedAverage = 0.0;
            double sumOfPayments = 0.0;
            foreach (Loan.Payment payment in loan.Payments)
            {
                sumOfPayments += payment.Amount;
                weightedAverage += YearsTo(payment.Date, loan) * payment.Amount;
            }
            if (loan.Commitment != 0.0)
                duration = weightedAverage / sumOfPayments;
            return duration;
        }
    }
}
