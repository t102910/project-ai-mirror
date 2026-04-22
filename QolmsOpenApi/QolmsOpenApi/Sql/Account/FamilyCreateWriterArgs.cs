using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 家族アカウント作成Writer 引数
    /// </summary>
    public class FamilyCreateWriterArgs: QsDbWriterArgsBase<MGF_NULL_ENTITY>
    {
        public Guid ParentAccountKey { get; set; } = Guid.Empty;

        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string FamilyNameKana { get; set; }
        public string GivenNameKana { get; set; }
        public string NickName { get; set; }
        public byte Sex { get; set; }
        public DateTime BirthDate { get; set; }

        public Guid PhotoKey { get; set; }

    }
}