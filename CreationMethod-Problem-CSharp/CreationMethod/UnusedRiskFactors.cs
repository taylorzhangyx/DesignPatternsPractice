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
	public class UnusedRiskFactors
	{
		private static UnusedRiskFactors rf = new UnusedRiskFactors();
		protected static double[] factors = {0.00, 0.0012, 0.019, 0.023, 0.029};

		private UnusedRiskFactors()
		{
		}

		public static UnusedRiskFactors GetFactors()
		{
			return rf;
		}

		public double ForRating(int customerRating)
		{
			return factors[customerRating];
		}
	}
}
