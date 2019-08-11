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
	public class CapitalStrategyRCTL : CapitalStrategy
	{
		public override double Duration(Loan loan)
		{
			return WeightedAverageDuration(loan) + YearsBetween(loan.Maturity, loan.Expiry);
		}

		protected double YearsBetween(DateTime startDate, DateTime endDate)
		{
			return endDate.Subtract(startDate).Days / DAYS_PER_YEAR;
		}
	}
}
