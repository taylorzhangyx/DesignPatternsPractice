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
	public class CapitalStrategyRevolver : CapitalStrategy
	{
		public override double Capital(Loan loan)
		{
			return
				(loan.OutstandingRiskAmount * Duration(loan) * RiskFactorFor(loan))
				+ (loan.UnusedRiskAmount * Duration(loan) * UnusedRiskFactor(loan));
		}

		private double UnusedRiskFactor(Loan loan)
		{
			return UnusedRiskFactors.GetFactors().ForRating(loan.RiskRating);
		}
	}
}
