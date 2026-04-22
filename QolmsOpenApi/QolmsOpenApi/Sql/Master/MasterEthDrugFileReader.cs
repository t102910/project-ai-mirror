using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Sql
{
    /// <summary>
    /// 医薬品ファイル情報Reader
    /// </summary>
    public class MasterEthDrugFileReader : QsDbReaderBase, IQsDbDistributedReader<QH_ETHDRUGFILE_MST, MasterEthDrugFileReaderArgs, MasterEthDrugFileReaderResults>
    {
        /// <summary>
        /// トランザクション実行
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public MasterEthDrugFileReaderResults ExecuteByDistributed(MasterEthDrugFileReaderArgs args)
        {
            var result = new MasterEthDrugFileReaderResults
            {
                IsSuccess = false
            };

            var entity = GetEntity(args.FileKey);

            if(entity != null && entity.IsKeysValid())
            {
                result.Result = new List<QH_ETHDRUGFILE_MST> { entity };
                result.IsSuccess = true;
            }

            return result;
        }

        QH_ETHDRUGFILE_MST GetEntity(Guid fileKey)
        {
            var sql = @"
                Select *
                From QH_ETHDRUGFILE_MST
                Where DELETEFLAG = 0
                And FILEKEY = @p1;
            ";

            using (var con = QsDbManager.CreateDbConnection<QH_ETHDRUGFILE_MST>())
            {
                var parameters = new List<DbParameter>
                {
                    CreateParameter(con, "@p1",fileKey)
                };

                con.Open();

                return ExecuteReader<QH_ETHDRUGFILE_MST>(
                    con, 
                    null, 
                    CreateCommandText(con, sql), 
                    parameters
                ).FirstOrDefault();
            }
        }
    }
}