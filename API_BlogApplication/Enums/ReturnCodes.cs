using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_BlogApplication.Enums
{
    public enum ReturnCodes
    {
		ParameterError = 0,
		VerifySecretKeySucceeded = 2,
		VerifySecretKeyError = 3,
		DataCreateSucceeded = 100,
		DataUpdateSucceeded = 102,
		DataRemoveSucceeded = 104,
		DataGetSucceeded = 106,
		DataCreateFailed = 200,
		DataUpdateFailed = 203,
		DataRemoveFailed = 206,
		DataGetFailed = 207,
		DataGetFailedWithNoData = 209,
	}
}
