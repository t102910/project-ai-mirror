using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MGF.QOLMS.QolmsOpenApi.Controllers;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsCryptV1;

namespace QolmsOpenApi.Tests
{
    [TestClass]
    public class TestNoticeControllers
    {
        private const string EXECUTOR = "eizk0BvAjplRIljBpKm57z5e2bqQ2/HkJXUI7YRzMUYlefeWoG0dLl9K96DRQ/dr";
        private const string EXECUTORNAME = "xbcXXzuwm9GMF/bu8K9ujA==";

        


        //[TestMethod]
        //public void PostWaitingListWrite_Success()
        //{
        //    var args = new QoTisWaitingListWriteApiArgs()
        //    {
        //        ApiType = QoApiTypeEnum.TisWaitingListWrite.ToString(),
        //        ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
        //        DataType ="1",
        //        FacilityKey = "11dc3f5656524d0891471575c1723edb",
        //        LinkageSystemNo ="27004",
        //        WaitingListN = new List<QoApiWaitingListItem>() { new QoApiWaitingListItem() { 
        //                                                             DeleteFlag =bool.FalseString,
        //                                                             DepartmentCode ="03",
        //                                                             DepartmentName ="ないか",
        //                                                             Detail =new QoApiWaitingDetailItem(){
        //                                                                 DepartmentName ="ないか",
        //                                                                 DoctorCode ="02",
        //                                                                 DoctorName ="いしゃ",
        //                                                                 DosingSlipNo ="",
        //                                                                 DosingSlipType ="",
        //                                                                 InOutType ="1",
        //                                                                 MedicalActCode ="",
        //                                                                 MedicalActName ="",
        //                                                                 RoomCode ="",
        //                                                                 RoomName ="",
        //                                                                 SameDaySequence ="0"
        //                                                             },
        //                                                             ForeignKey ="151577780",
        //                                                             LinkageSystemId ="00000051",
        //                                                             ReceptionNo ="00001",
        //                                                             ReceptionTime ="1700",
        //                                                             ReservationNo ="",
        //                                                             ReservationTime ="",
        //                                                             StatusType ="25",
        //                                                             WaitingDate =DateTime.Now.ToApiDateString()
        //                                                        } 
        //        }, 
        //        ExecutorName = EXECUTORNAME,
        //        Executor = EXECUTOR
        //    };

        //    var controller = new TisController();
        //    var result = controller.PostTisWaitingListWrite(args);
        //    Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);

        //}
        //[TestMethod]
        //public void PostWaitingListRead_Success()
        //{
        //    var args = new QoTisWaitingListReadApiArgs()
        //    {
        //        ApiType = QoApiTypeEnum.TisWaitingListRead.ToString(),
        //        ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
        //        DataType="1",
        //        FacilityKeyReference =new QsCrypt(QsCryptTypeEnum.QolmsWeb).EncryptString("11dc3f5656524d0891471575c1723edb"),
        //        TargetDate =DateTime.Now.Date.ToApiDateString(),
        //        ActorKey = "GDQatN2U5kLXBHmUMMEBrljx1Hdhs0q+ZdBexHjZeM90UqXaYklX4R74srppjAc6",
        //        ExecutorName = EXECUTORNAME,
        //        Executor = EXECUTOR
        //    };

        //    var controller = new TisController();
        //    var result = controller.PostTisWaitingListRead(args);
        //    Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
        //    Assert.AreEqual(string.IsNullOrWhiteSpace(result.StatusType ), false);
        //    Assert.AreEqual(string.IsNullOrWhiteSpace(result.Detail.DepartmentName ), false);
        //    Assert.AreEqual(string.IsNullOrWhiteSpace(result.WaitingDate ), false);
         
        //}

    }
}
