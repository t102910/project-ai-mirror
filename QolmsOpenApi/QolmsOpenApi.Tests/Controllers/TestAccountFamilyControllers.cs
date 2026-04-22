using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MGF.QOLMS.QolmsOpenApi.Controllers;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsApiCoreV1;
using System.Linq;

namespace QolmsOpenApi.Tests
{
    [TestClass]
    public class TestAccountFamilyControllers
    {
        private const string EXECUTOR = "eizk0BvAjplRIljBpKm57z5e2bqQ2/HkJXUI7YRzMUYlefeWoG0dLl9K96DRQ/dr";
        private const string EXECUTORNAME = "xbcXXzuwm9GMF/bu8K9ujA==";
        /// <summary>
        /// 子アカウントリスト取得を確認
        /// </summary>
        [TestMethod]
        public void PostListRead_Success()
        {
            //ログイン成功したらTokenが返ってくる
            var args = new QoAccountFamilyListReadApiArgs() {
                ApiType = QoApiTypeEnum.AccountFamilyListRead.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName =EXECUTORNAME ,
                Executor = EXECUTOR ,
                ActorKey = "FXnl8lz2fQYuWOOGzKWyk/FFoAuEcrY4TIzjxa0duyYn3bITs5XOmc7GKzK3wJQS",                    //テスト用（本来はトークンからいれられる）
            };
            var controller = new AccountFamilyController();
            var result =controller.PostListRead(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
            Assert.AreEqual(result.AccountN.Any(), true);
        }
        /// <summary>
        /// 子アカウント追加を確認
        /// </summary>
        [TestMethod]
        public void PostAdd_Success()
        {
            //子アカウント追加成功
            var args = new QoAccountFamilyAddApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountFamilyAdd.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
                ActorKey = "FXnl8lz2fQYuWOOGzKWyk/FFoAuEcrY4TIzjxa0duyYn3bITs5XOmc7GKzK3wJQS",                    //テスト用（本来はトークンからいれられる）
                Account = new QoApiAccountFamilyInputItem()
                {
                    Birthday = DateTime.Now.ToApiDateString(),
                    FamilyName = "てすと",
                    FamilyNameKana = "テスト",
                    GivenName = "こ",
                    GivenNameKana = "コ",
                    Sex = "1",
                    Tel = string.Empty,
                }
            };
            var controller = new AccountFamilyController();
            var result = controller.PostAdd(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
        }
        [TestMethod]
        public void PostAdd_Error()
        {
            //子アカウント追加失敗
            var args = new QoAccountFamilyAddApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountFamilyAdd.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
                ActorKey = "FXnl8lz2fQYuWOOGzKWyk/FFoAuEcrY4TIzjxa0duyYn3bITs5XOmc7GKzK3wJQS",                    //テスト用（本来はトークンからいれられる）
                
            };
            var controller = new AccountFamilyController();
            var result = controller.PostAdd(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), false);
        }

        /// <summary>
        /// 子アカウント削除を確認
        /// </summary>
        [TestMethod]
        public void PostDelete_Success()
        {
            //子アカウント削除成功
            var args = new QoAccountFamilyDeleteApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountFamilyDelete.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME ,
                Executor = EXECUTOR ,
                ActorKey = "FXnl8lz2fQYuWOOGzKWyk/FFoAuEcrY4TIzjxa0duyYn3bITs5XOmc7GKzK3wJQS",                    //テスト用（本来はトークンからいれられる）
                AccountKeyReference = "ce76bfea49c215d6de53821fbf18e0bcc60c4fcf13677810a4b04acd76ced9652d1b3bb9334e41249b92fd118662500a84c75a5757790e93532cbe1a190e1f61",
            };
            var controller = new AccountFamilyController();
            var result = controller.PostDelete(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
        }

        
        /// <summary>
        /// 顔写真取得を確認します。
        /// </summary>
        [TestMethod]
        public void PostParsonPhotoRead_Success()
        {
            //顔写真取得
            var args = new QoAccountFamilyPersonPhotoReadApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountFamilyPersonPhotoRead.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
                PersonPhotoReference ="",   
                ActorKey = "FXnl8lz2fQYuWOOGzKWyk/FFoAuEcrY4TIzjxa0duyYn3bITs5XOmc7GKzK3wJQS"                    //テスト用（本来はトークンからいれられる）
            };
            var controller = new AccountFamilyController();
            var result = controller.PostPersonPhotoRead(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
        }

        /// <summary>
        /// 顔写真保存を確認します。
        /// </summary>
        [TestMethod]
        public void PostPersonPhotoWrite_Success()
        {
            //顔写真保存
            var args = new QoAccountFamilyPersonPhotoWriteApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountFamilyPersonPhotoWrite.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
                PersonPhoto = new QoApiFileItem()
                {
                    ContentType ="",
                    Data ="",
                    FileKeyReference ="",
                    OriginalName ="",
                    Sequence="1"
                },
                ActorKey = "FXnl8lz2fQYuWOOGzKWyk/FFoAuEcrY4TIzjxa0duyYn3bITs5XOmc7GKzK3wJQS"                    //テスト用（本来はトークンからいれられる）
            };
            var controller = new AccountFamilyController();
            var result = controller.PostPersonPhotoWrite(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
            Assert.AreEqual(string.IsNullOrWhiteSpace(result.PersonPhotoReference), false);
        }
        [TestMethod]
        public void PostPersonPhotoWrite_Error()
        {
            //無効
            //顔写真保存
            var args = new QoAccountFamilyPersonPhotoWriteApiArgs()
            {
                ApiType = QoApiTypeEnum.AccountFamilyPersonPhotoWrite.ToString(),
                ExecuteSystemType = QsApiSystemTypeEnum.QolmsOpenApi.ToString(),
                ExecutorName = EXECUTORNAME,
                Executor = EXECUTOR,
                ActorKey = "FXnl8lz2fQYuWOOGzKWyk/FFoAuEcrY4TIzjxa0duyYn3bITs5XOmc7GKzK3wJQS"                    //テスト用（本来はトークンからいれられる）
            };
            var controller = new AccountFamilyController();
            var result = controller.PostPersonPhotoWrite(args);
            Assert.AreEqual(result.IsSuccess.TryToValueType(false), true);
        }
       
    }
}
