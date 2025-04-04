﻿using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Wrpg;

public static class CustomTypedResults
{
    public static BadRequest<ProblemDetails> BadRequest(string problemDetail) =>
        TypedResults.BadRequest(new ProblemDetails { Detail = problemDetail });
}