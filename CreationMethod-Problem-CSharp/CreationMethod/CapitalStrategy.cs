/// ***************************************************************************
/// Copyright (c) 2009, Industrial Logic, Inc., All Rights Reserved.
///
/// This code is the exclusive property of Industrial Logic, Inc. It may ONLY be
/// used by students during Industrial Logic's workshops or by individuals
/// who are being coached by Industrial Logic on a project.
///
/// This code may NOT be copied or used for any other purpose without the prior
/// written consent of Industrial Logic, Inc.
/// ****************************************************************************

//$CopyrightHeader()$

using System;

namespace IndustrialLogic.CreationMethod
{
	public abstract class CapitalStrategy
	{
		protected const int MILLIS_PER_DAY = 86400000;
		protected const int DAYS_PER_YEAR = 365;

		public virtual double Capital(Loan loan)
		{
			return loan.Commitment * Duration(loan) * RiskFactorFor(loan);
		}

		protected double RiskFactorFor(Loan loan)
		{
			return RiskFactor.GetFactors().ForRating(loan.RiskRating);
		}

		protected double YearsTo(DateTime endDate, Loan loan)
		{
			DateTime beginDate = (loan.Today == Loan.NO_DATE ? loan.StartDate : loan.Today);
			return endDate.Subtract(beginDate).Days / DAYS_PER_YEAR;
		}

		public virtual double Duration(Loan loan)
		{
			return YearsTo(loan.Expiry, loan);
		}

		protected double WeightedAverageDuration(Loan loan)
		{
			double duration = 0.0;
			double weightedAverage = 0.0;
			double sumOfPayments = 0.0;

			foreach (Payment payment in loan.Payments)
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
