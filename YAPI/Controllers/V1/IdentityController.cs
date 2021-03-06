﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Yapi.Contracts;
using Yapi.Contracts.External.Facebook;
using Yapi.Contracts.V1;
using Yapi.Contracts.V1.Requests;
using Yapi.Contracts.V1.Responses;
using YAPI.Filters;
using YAPI.Services;

namespace YAPI.Controllers.V1
{
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService identityService;

        public IdentityController(IIdentityService identityService)
        {
            this.identityService = identityService;
        }

        [HttpPost(ApiRoutes.Identity.Register)]
        // [ApiKeyAuth]//herkeşler register olamaz only those who got the apikeys are allowed
        public async Task<IActionResult> Register([FromBody]RegistrationRequest request)
        {
            //buna gerek yok ValidationFilter.cs ile model state kontrolü mw seviyesinde yapılıyor
            //if (!ModelState.IsValid)
            //    return BadRequest(new AuthenticationResponse() { Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage)) });

            var registrationResponse = await identityService.RegisterAsync(request.Email, request.Password);
            if (!registrationResponse.Success)
                return BadRequest(registrationResponse);

            return Ok(registrationResponse);
        }

        [HttpPost(ApiRoutes.Identity.Login)]
        public async Task<IActionResult> Login([FromBody]LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthenticationResponse() { Errors = ModelState.Values.SelectMany(x => x.Errors.Select(e => e.ErrorMessage)) });

            var loginResponse = await identityService.LoginAsync(request.Email, request.Password);
            if (!loginResponse.Success)
                return BadRequest(loginResponse);

            return Ok(loginResponse);
        }

        /// <summary>
        /// the client needs to store the expiry date in the local storage and on every request you need to check if it’s in the past. If it is then you use a middleware to call the refresh endpoint and get a new set of keys.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost(ApiRoutes.Identity.Refresh)]
        public async Task<IActionResult> Refresh(RefreshTokenRequest request)
        {
            var refreshResponse = await identityService.RefreshTokenAsync(request.Token,request.RefreshToken);
            if (!refreshResponse.Success)
                return BadRequest(refreshResponse);

            return Ok(refreshResponse);
        }


        [HttpPost(ApiRoutes.Identity.FaceAuth)]
        public async Task<IActionResult> Login([FromBody]UserFacebookAuthRequest request)
        {
            //ModelState ValidationFilter
            var loginResponse = await identityService.LoginWithFacebookAsync(request.AccessToken);
            if (!loginResponse.Success)
                return BadRequest(loginResponse);

            return Ok(loginResponse);
        }
    }
}