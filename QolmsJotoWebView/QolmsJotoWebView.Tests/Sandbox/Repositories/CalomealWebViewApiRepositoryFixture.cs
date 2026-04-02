using MGF.QOLMS.QolmsCalomealWebViewApiCoreV1;
using MGF.QOLMS.QolmsJotoWebView.Models;
using MGF.QOLMS.QolmsJotoWebView.Repositories;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace QolmsJotoWebView.Tests.Repositories
{
    [TestFixture]
    public class CalomealWebViewApiRepositoryTests
    {
        private ICalomealWebViewApiRepository _repository;
        //private Mock<IAccessLogWorker> _mockAccessLogWorker;
        //private Mock<IQsCalomealWebViewApiManager> _mockApiManager;

        [SetUp]
        public void SetUp()
        {
            _repository = new CalomealWebViewApiRepository();

        }

        #region 

        [Test]
        public void トークンの発行が正常にできる()
        {
            //var ret = _repository.GetNewToken(code);
        
        }

        #endregion
    }
}
