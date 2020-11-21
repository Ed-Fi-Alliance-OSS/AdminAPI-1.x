#if NET48
// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using EdFi.Ods.AdminApp.Web.ActionFilters;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EdFi.Ods.AdminApp.Management.Tests.ActionFilters
{
    class RequireSecureConnectionFilterTests
    {
        [Test]
        public void OnAuthorization_nocontext_should_throw()
        {
            var filter = new RequreSecureConnectionFilter();
            Assert.Throws<ArgumentNullException>(() => filter.OnAuthorization(null));
        }

        [Test]
        public void OnAuthorization_should_not_redirect_local_requests()
        {
            var context = GetTestFilterContext(true);
            var filter = new RequreSecureConnectionFilter();
            filter.OnAuthorization(context);
            var redirectResult = context.Result as RedirectResult;
            Assert.Null(redirectResult);
        }

        [Test]
        public void Nonlocal_requests_should_require_secure_connection()
        {
            var context = GetTestFilterContext(false);
            var filter = new RequreSecureConnectionFilter();
            var exception = Assert.Throws<InvalidOperationException>(() => filter.OnAuthorization(context));
            exception.Message.ShouldBe("The requested resource can only be accessed via SSL.");
        }

        private static AuthorizationContext GetTestFilterContext(bool isLocalConnection)
        {
            var request = new Mock<HttpRequestBase>();
            request.Setup(x => x.IsLocal).Returns(isLocalConnection);

            var context = new Mock<HttpContextBase>();
            context.Setup(x => x.Request).Returns(request.Object);

            var controller = new Mock<ControllerBase>();
            var actionDescriptor = new Mock<ActionDescriptor>();
            var controllerContext = new ControllerContext(context.Object, new RouteData(), controller.Object);
            return new AuthorizationContext(controllerContext, actionDescriptor.Object);
        }
    }
}
#endif
