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

namespace IndustrialLogic.CreationMethod
{
	public class RiskAdjustedCapitalStrategy : CapitalStrategy
	{
		private readonly double riskAdjustment;

		public RiskAdjustedCapitalStrategy(double riskAdjustment)
		{
			this.riskAdjustment = riskAdjustment;
		}

		public override double Capital(Loan loan)
		{
			return loan.Commitment * riskAdjustment * Duration(loan) * RiskFactorFor(loan);
		}

		public override double Duration(Loan loan)
		{
			return WeightedAverageDuration(loan);
		}
	}
}
