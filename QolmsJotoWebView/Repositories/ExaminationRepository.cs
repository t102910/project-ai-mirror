using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsJotoWebView.Repositories
{
    interface IExaminationRepository
    {
        QjNoteExaminationReadApiResults ExecuteNoteExaminationReadApi(QolmsJotoModel mainModel);

        byte[] ExecuteNoteExaminationPdfReadApi(QolmsJotoModel mainModel, Guid fileKey, int linkageSystemNo, string linkageSystemId, DateTime recordDate, Guid facilityKey, ref string refOriginalName, ref string refContentType);
    }

    public class ExaminationRepository: IExaminationRepository
    {
        public QjNoteExaminationReadApiResults ExecuteNoteExaminationReadApi(QolmsJotoModel mainModel)
        {
            var apiArgs = new QjNoteExaminationReadApiArgs(
                QjApiTypeEnum.NoteExaminationRead,
                QsApiSystemTypeEnum.QolmsJoto,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString()
            };

            var apiResults = QsApiManager.ExecuteQolmsJotoApi<QjNoteExaminationReadApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey2
            );

            if (apiResults.IsSuccess.TryToValueType(false))
            {
                return apiResults;
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format(
                        "{0} API の実行に失敗しました。",
                        QsApiManager.GetQolmsJotoApiName(apiArgs)
                    )
                );
            }
        }

        public byte[] ExecuteNoteExaminationPdfReadApi(
            QolmsJotoModel mainModel,
            Guid fileKey,
            int linkageSystemNo,
            string linkageSystemId,
            DateTime recordDate,
            Guid facilityKey,
            ref string refOriginalName,
            ref string refContentType)
        {
            refOriginalName = string.Empty;
            refContentType = string.Empty;

            byte[] result = null;

            var apiArgs = new QjNoteExaminationPdfReadApiArgs(
                QjApiTypeEnum.NoteExaminationPdfRead,
                QsApiSystemTypeEnum.QolmsJoto,
                mainModel.ApiExecutor,
                mainModel.ApiExecutorName
            )
            {
                ActorKey = mainModel.AuthorAccount.AccountKey.ToApiGuidString(),
                FileKey = fileKey.ToApiGuidString(),
                LinkageSystemNo = linkageSystemNo.ToString(),
                LinkageSystemId = linkageSystemId,
                RecordDate = recordDate.ToApiDateString(),
                FacilityKey = facilityKey.ToApiGuidString(),
                DataType = Convert.ToByte(QjExaminationDataTypeEnum.OverallAssessmentPdf).ToString()
            };

            var apiResults = QsApiManager.ExecuteQolmsJotoApi<QjNoteExaminationPdfReadApiResults>(
                apiArgs,
                mainModel.SessionId,
                mainModel.ApiAuthorizeKey2
            );

            if (apiResults.IsSuccess.TryToValueType(false) &&
                string.Compare(Path.GetExtension(apiResults.OriginalName), ".pdf", true) == 0 &&
                string.Compare(apiResults.ContentType, "application/pdf", true) == 0 &&
                !string.IsNullOrWhiteSpace(apiResults.Data))
            {
                refOriginalName = apiResults.OriginalName;
                refContentType = apiResults.ContentType;
                result = Convert.FromBase64String(apiResults.Data);
            }
            else
            {
                throw new InvalidOperationException(
                    string.Format(
                        "{0} API の実行に失敗しました。",
                        QsApiManager.GetQolmsJotoApiName(apiArgs)
                    )
                );
            }

            return result;
        }

    }
}