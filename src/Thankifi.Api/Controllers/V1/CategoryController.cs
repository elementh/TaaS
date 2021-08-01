﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Thankifi.Api.Model.V1.Requests.Category;
using Thankifi.Api.Model.V1.Responses;

namespace Thankifi.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1")]
    [Produces("application/json")]
    [Route("api/v{v:apiVersion}/[controller]")]
    public class CategoryController : ControllerBase
    {
        /// <summary>
        /// Retrieve a paginated list of all the available categories. Thanks!
        /// </summary>
        [HttpGet(Name = nameof(RetrieveAllCategories))]
        [ProducesResponseType(typeof(IEnumerable<CategoryViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> RetrieveAllCategories(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieve a detail view of a category and a paginated list of gratitudes for the specified category id.
        /// Optionally specify a subject, a signature, flavours and languages. Thanks!
        /// </summary>
        [HttpGet("{id:guid:required}", Name = nameof(RetrieveByCategoryId), Order = 1)]
        [ProducesResponseType(typeof(CategoryDetailViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RetrieveByCategoryId([FromRoute] Guid id, RetrieveByCategoryQueryParameters query, CancellationToken cancellationToken)
        {
            return Ok(Guid.NewGuid());
        }

        /// <summary>
        /// Retrieve a detail view of a category and a paginated list of gratitudes for the specified category slug.
        /// Optionally specify a subject, a signature, flavours and languages. Thanks!
        /// </summary>
        [HttpGet("{slug:required}", Name = nameof(RetrieveByCategorySlug), Order = 2)]
        [ProducesResponseType(typeof(CategoryDetailViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RetrieveByCategorySlug([FromRoute] string slug, RetrieveByCategoryQueryParameters query, CancellationToken cancellationToken)
        {
            return Ok("slug");
        }
    }
}