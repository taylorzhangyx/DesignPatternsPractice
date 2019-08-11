
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

using System;

namespace IndustrialLogic.Strategy
{
    public abstract class CapitalStrategy
    {
        private const int DAYS_PER_YEAR = 365;

        protected abstract double RiskAmount(Loan loan);

        protected double RiskFactorFor(Loan loan)
        {
            return RiskFactor.GetFactors().ForRating(loan.RiskRating);
        }

        protected double YearsTo(DateTime endDate, Loan loan)
        {
            return (endDate.Subtract(loan.Start).Days / DAYS_PER_YEAR);
        }

        public virtual double Duration(Loan loan)
        {
            return YearsTo(loan.Expiry, loan);
        }

        public virtual double Capital(Loan loan)
        {
            return RiskAmount(loan) * Duration(loan) * RiskFactorFor(loan);
        }
    }
}
