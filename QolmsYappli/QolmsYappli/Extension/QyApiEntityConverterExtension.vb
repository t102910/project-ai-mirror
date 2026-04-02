Imports System.Globalization
Imports System.Runtime.CompilerServices
Imports MGF.QOLMS.QolmsApiCoreV1
Imports MGF.QOLMS.QolmsApiEntityV1
Imports MGF.QOLMS.QolmsDbEntityV1

''' <summary>
''' 「沖縄セルラー Yappli」で使用する API 用エンティティクラスを、
''' 異なる形式のエンティティクラスへ変換するための拡張機能を提供します。
''' </summary>
''' <remarks></remarks>
Friend Module QyApiEntityConverterExtension

#Region "QhApiExerciseItem"

    ''' <summary>
    ''' API 用の運動の情報クラスを、
    ''' 運動の情報クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 運動の情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToExerciseItem(target As QhApiExerciseItem) As ExerciseItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New ExerciseItem() With {
            .ExerciseDate = Date.Parse(target.ExerciseDate),
            .ExerciseType = target.ExerciseType,
            .Sequence = Integer.Parse(target.Sequence),
            .ExerciseName = target.ExerciseName,
            .Calorie = Short.Parse(target.Calorie),
            .PhotoKey = target.PhotoKey
        }

    End Function

#End Region

#Region "QhApiMealItem"

    ''' <summary>
    ''' API 用の食事の情報クラスを、
    ''' 食事の情報クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 食事の情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToMealItem(target As QhApiMealItem) As MealItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New MealItem() With {
            .MealDate = Date.Parse(target.MealDate),
            .MealType = target.MealType,
            .Sequence = Integer.Parse(target.Sequence),
            .MealName = target.MealName,
            .Calorie = Short.Parse(target.Calorie),
            .PhotoKey = Guid.Parse(target.PhotoKey),
            .FoodN = target.FoodN.ToList().ConvertAll(
                Function(i) New FoodItem() With {
                    .label = i.label,
                    .probability = i.probability,
                    .calorie = i.calorie,
                    .protein = i.protein,
                    .lipid = i.lipid,
                    .carbohydrate = i.carbohydrate,
                    .salt_amount = i.salt_amount,
                    .available_carbohydrate = i.available_carbohydrate,
                    .fiber = i.fiber
                }
        )
        }

    End Function

#End Region

#Region "QhApiVitalValueItem"

    ''' <summary>
    ''' API 用のバイタル情報クラスを、
    ''' バイタル情報クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' バイタル情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToVitalValueItem(target As QhApiVitalValueItem) As VitalValueItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New VitalValueItem() With {
            .RecordDate = Date.Parse(target.RecordDate),
            .VitalType = target.VitalType.TryToValueType(QyVitalTypeEnum.None),
            .Value1 = Decimal.Parse(target.Value1),
            .Value2 = Decimal.Parse(target.Value2),
            .ConditionType = target.ConditionType.TryToValueType(QyVitalConditionTypeEnum.None)
        }

    End Function

#End Region

#Region "VitalValueInputModel"

    ''' <summary>
    ''' バイタル情報インプットモデルを、
    ''' API 用のバイタル情報クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元モデル。</param>
    ''' <param name="recordDate">編集日。</param>
    ''' <returns>
    ''' API 用のバイタル情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToApiVitalValueItem(target As VitalValueInputModel, recordDate As Date) As QhApiVitalValueItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元モデルがNull参照です。")

        Return New QhApiVitalValueItem() With {
            .RecordDate = Date.Parse(String.Format("#{0}/{1}/{2} {3}:{4}:00 {5}#", recordDate.Month, recordDate.Day, recordDate.Year, target.Hour, target.Minute, target.Meridiem)).ToApiDateString(),
            .VitalType = target.VitalType.ToString(),
            .Value1 = If(String.IsNullOrWhiteSpace(target.Value1), Decimal.MinValue, Decimal.Parse(target.Value1)).ToString(),
            .Value2 = If(String.IsNullOrWhiteSpace(target.Value2), Decimal.MinValue, Decimal.Parse(target.Value2)).ToString(),
            .ConditionType = If(String.IsNullOrWhiteSpace(target.ConditionType), QyVitalConditionTypeEnum.None, DirectCast([Enum].Parse(GetType(QyVitalConditionTypeEnum), target.ConditionType), QyVitalConditionTypeEnum)).ToString()
        }

    End Function

#End Region

#Region "QhApiHealthAgeValueItem"

    <Extension()>
    Public Function ToHealthAgeValueItem(target As QhApiHealthAgeValueItem) As HealthAgeValueItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New HealthAgeValueItem() With {
            .RecordDate = Date.Parse(target.RecordDate),
            .HealthAgeValueType = target.HealthAgeValueType.TryToValueType(QyHealthAgeValueTypeEnum.None),
            .Value = Decimal.Parse(target.Value),
            .Comparison = Decimal.Parse(target.Comparison),
            .IsRedCode = target.IsRedCode.TryToValueType(False),
            .Label = target.Label,
            .SortOrder = Integer.Parse(target.SortOrder)
        }

    End Function

#End Region

#Region "QhApiHealthAgeReportItem"

    <Extension()>
    Public Function ToHealthAgeReportItem(target As QhApiHealthAgeReportItem) As HealthAgeReportItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New HealthAgeReportItem() With {
            .HealthAgeReportType = target.HealthAgeReportType.TryToValueType(QyHealthAgeReportTypeEnum.None),
            .HealthAgeValueN = target.HealthAgeValueN.ConvertAll(Function(i) i.ToHealthAgeValueItem()),
            .Deviance = Decimal.Parse(target.Deviance)
        }

    End Function

#End Region

#Region "QhApiHealthAgeAdviceGoalItem"

    <Extension()>
    Public Function ToHealthAgeAdviceGoalItem(target As QhApiHealthAgeAdviceGoalItem) As HealthAgeAdviceGoalItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New HealthAgeAdviceGoalItem() With {
            .Name = target.Name,
            .Old = Decimal.Parse(target.Old),
            .Goal = Decimal.Parse(target.Goal)
        }

    End Function

#End Region

#Region "QhApiHealthAgeAdviceItem"

    <Extension()>
    Public Function ToHealthAgeAdviceItem(target As QhApiHealthAgeAdviceItem) As HealthAgeAdviceItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New HealthAgeAdviceItem() With {
            .Title = target.Title,
            .Based = target.Based,
            .DetailN = target.DetailN,
            .GoalN = target.GoalN.ConvertAll(Function(i) i.ToHealthAgeAdviceGoalItem())
        }

    End Function

#End Region

#Region "QhApiMedicalInstitutionItem"

    ''' <summary>
    ''' API 用の医療機関情報クラスを、
    ''' 医療機関情報クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 医療機関情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToMedicalInstitutionItem(target As QhApiMedicalInstitutionItem) As MedicalInstitutionItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New MedicalInstitutionItem() With {
            .CodeNo = Integer.Parse(target.CodeNo),
            .InstitutionName = target.InstitutionName,
            .KanaName = target.KanaName,
            .PostalCode = target.PostalCode,
            .Address = target.Address,
            .DepartmentN = target.DepartmentN,
            .OptionFlags = Integer.Parse(target.OptionFlags)
            }

    End Function

#End Region

#Region "QhApiMedicalInstitutionAcceptedItem"

    ''' <summary>
    ''' API 用の医療機関診療時間クラスを、
    ''' 医療機関情報クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 医療機関情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToMedicalInstitutionAcceptedItem(target As QhApiMedicalInstitutionAcceptedItem) As MedicalInstitutionAcceptedItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New MedicalInstitutionAcceptedItem() With {
            .AccepteDays = target.AccepteDays.ConvertAll(Function(i) Boolean.Parse(i)),
            .AcceptedStart = target.AcceptedStart,
            .AcceptedEnd = target.AcceptedEnd
            }

    End Function

#End Region

#Region "QhApiMedicalOfficeHouers"

    ''' <summary>
    ''' API 用の医療機関診療時間クラスを、
    ''' 医療機関情報クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 医療機関情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToMedicalOfficeHouers(target As QhApiMedicalOfficeHouers) As MedicalOfficeHouers

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New MedicalOfficeHouers() With {
            .CodeNo = Integer.Parse(target.CodeNo),
            .DayOfWeek = Integer.Parse(target.DayOfWeek),
            .AcceptedTimeNo = Integer.Parse(target.AcceptedTimeNo),
            .AcceptedStart = target.AcceptedStart,
            .AcceptedEnd = target.AcceptedEnd
            }

    End Function

#End Region

#Region "QhApiPaymentLogItem"

    ''' <summary>
    ''' API 用の支払い（定期課金）ログ情報クラスを、
    ''' 支払い（定期課金）ログ情報クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 支払い（定期課金）ログ情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToPaymentLogItem(target As QhApiPaymentLogItem) As PaymentLogItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスが Null 参照です。")

        Return New PaymentLogItem() With {
            .PaymentYear = Integer.Parse(target.PaymentYear),
            .PaymentMonth = Integer.Parse(target.PaymentMonth),
            .PaymentDate = Date.Parse(target.PaymentDate),
            .PaymentType = Byte.Parse(target.PaymentType),
            .Amount = Decimal.Parse(target.Amount),
            .StatusCode = target.StatusCode
        }

    End Function

#End Region

#Region "QhApiDatachargeEventIdItem"

    ''' <summary>
    ''' API 用のデータチャージイベントIDクラスを、
    ''' データチャージイベントIDクラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 医療機関情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToDatachargeEventIdItem(target As QhApiDatachargeEventIdItem) As DatachargeEventIdItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New DatachargeEventIdItem() With {
            .EventId = target.EventId,
            .Size = Integer.Parse(target.Size),
            .Point = Integer.Parse(target.Point),
            .DispName = target.DispName
            }

    End Function

#End Region

#Region "QhApiDatachargeHistItem"

    ''' <summary>
    ''' API 用のデータチャージ履歴クラスを、
    ''' データチャージ履歴クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 医療機関情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToDatachargeHistItem(target As QhApiDatachargeHistItem) As DatachargeHistItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New DatachargeHistItem() With {
            .ActionDate = Date.Parse(target.ActionDate),
            .Size = Integer.Parse(target.Size),
            .Point = Integer.Parse(target.Point),
            .DispName = target.DispName
            }

    End Function

#End Region

#Region "QhApiCouponItem"

    ''' <summary>
    ''' API 用のクーポン情報クラスを、
    ''' ポイント交換履歴クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' クーポン情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToCouponItem(target As QhApiCouponItem) As CouponItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New CouponItem() With {
            .CouponType = Byte.Parse(target.CouponType),
            .Point = Integer.Parse(target.Point),
            .DispName = target.DispName,
            .RestCount = Integer.Parse(target.RestCount)
            }

    End Function

#End Region

#Region "QhApiPointExchangeHistItem"

    ''' <summary>
    ''' API 用のポイント交換履歴クラスを、
    ''' ポイント交換履歴クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 医療機関情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToPointExchangeHistItem(target As QhApiPointExchangeHistItem) As PointExchangeHistItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New PointExchangeHistItem() With {
            .IssueDate = Date.Parse(target.IssueDate),
            .ExpirationDate = Date.Parse(target.ExpirationDate),
            .CouponType = Byte.Parse(target.CouponType),
            .CouponId = target.CouponId,
            .DispName = target.DispName,
            .Point = Integer.Parse(target.Point)
            }

    End Function

#End Region

#Region "QhApiSearchMstItem"

    ''' <summary>
    ''' API 用の医療機関情報クラスを、
    ''' 医療機関情報クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 医療機関情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToSearchMstItem(target As QhApiSearchMstItem) As SearchMstItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New SearchMstItem() With {
            .Key = target.Key,
            .SubKey = target.SubKey,
            .Value = target.Value,
            .SubValue = target.SubValue
            }

    End Function

#End Region

#Region "QhApiAuWalletPointItem"

    ''' <summary>
    ''' API 用のau WALLET ポイントクラスを、
    ''' au WALLET ポイントクラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 医療機関情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToAuWalletPointItem(target As QhApiAuWalletPointItem) As AuWalletPointItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New AuWalletPointItem() With {
            .AuWalletPointItemId = target.AuWalletPointItemId,
            .Point = Integer.Parse(target.Point),
            .DispName = target.DispName
            }

    End Function

#End Region

#Region "QhApiAuWalletPointItem"

    ''' <summary>
    ''' API 用のau WALLET ポイント交換履歴クラスを、
    ''' au WALLET ポイント交換履歴クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 医療機関情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToAuWalletPointHistItem(target As QhApiAuWalletPointHistItem) As AuWalletPointHistItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New AuWalletPointHistItem() With {
            .ActionDate = Date.Parse(target.ActionDate),
            .Point = Integer.Parse(target.Point),
            .DispName = target.DispName
            }

    End Function

#End Region

#Region "QhApiAmazonGiftCardItem"

    ''' <summary>
    ''' API 用のAmazonギフト券情報クラスを、
    ''' Amazonギフト券クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 医療機関情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToAmazonGiftCardItem(target As QhApiAmazonGiftCardItem) As AmazonGiftCardItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New AmazonGiftCardItem() With {
            .GiftCardType = Byte.Parse(target.GiftcardType),
            .DemandPoint = Integer.Parse(target.DemandPoint),
            .GiftCardName = target.GiftcardName,
            .RestCount = Integer.Parse(target.RestCount)
            }

    End Function

#End Region

#Region "QhApiAmazonGiftCardHistItem"

    ''' <summary>
    ''' API 用のAmazonギフト券交換履歴クラスを、
    ''' Amazonギフト券交換履歴クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 医療機関情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToAmazonGiftCardHistItem(target As QhApiAmazonGiftCardHistItem) As AmazonGiftCardHistItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New AmazonGiftCardHistItem() With {
            .IssueDate = Date.Parse(target.IssueDate),
            .ExpirationDate = Date.Parse(target.ExpirationDate),
            .GiftCardType = Byte.Parse(target.GiftcardType),
            .GiftCardId = target.GiftcardId,
            .GiftCardName = target.GiftcardName,
            .DemandPoint = Integer.Parse(target.DemandPoint)
            }

    End Function

#End Region
    
#Region "QhApiCalomealTokenSetItem"

    ''' <summary>
    ''' API 用のカロミルトークン情報クラスを、
    ''' カロミルトークン クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToCalomealTokenSet(target As QhApiCalomealTokenSetItem) As CalomealAccessTokenSet

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New CalomealAccessTokenSet() With {
            .access_token = target.Token,
            .TokenExpires = target.TokenExpires.TryToValueType(Date.MinValue),
            .refresh_token = target.RefreshToken,
            .RefreshTokenExpires = target.RefreshTokenExpires.TryToValueType(Date.MinValue)
            }

    End Function

#End Region



#Region "QhApiExaminationSetItem"

    ''' <summary>
    ''' API 用の日付け、連番ごとの検査情報クラスを、
    ''' 日付け、連番ごとの検査情報クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 検査情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToExaminationSetItem(target As QhApiExaminationSetItem) As ExaminationSetItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New ExaminationSetItem() With {
           .Sequence = target.Sequence.TryToValueType(Integer.MinValue),
           .CacheKey = target.RecordDate + target.Sequence,
           .CategoryId = target.CategoryId.ToValueType(Of Integer)(),
           .RecordDate = target.RecordDate.ToValueType(Of Date)(),
           .LinkageSystemNo = target.LinkageSystemNo.ToValueType(Of Integer)(),
           .OrganizationKey = target.OrganizationKey,
           .OrganizationName = target.OrganizationName,
           .OrganizationTel = target.OrganizationTel,
           .ExaminationN = target.ExaminationN.ConvertAll(Function(i) i.ToExaminationItem()),
           .HealthAge = target.HealthAge.TryToValueType(Integer.MinValue),
           .ExaminationJudgementN = target.ExaminationJudgementN.ConvertAll(Function(i) i.ToExaminationJudgementItem()),
           .OverallAssessmentPdfN = target.OverallAssessmentPdfN.ConvertAll(Function(i) i.ToExaminationAssociatedFileItem())
       }

    End Function
#End Region

#Region "QhApiExaminationGroupItem"

    ''' <summary>
    ''' API 用の日付け、連番ごとの検査グループ情報クラスを、
    ''' 日付け、連番ごとの検査情報クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 検査グループ情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToExaminationGroupItem(target As QhApiExaminationGroupItem) As ExaminationGroupItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New ExaminationGroupItem() With {
            .GroupNo = target.GroupNo.ToValueType(Of Integer)(),
            .Name = target.Name,
            .Comment = If(Not String.IsNullOrWhiteSpace(target.Comment), target.Comment, String.Empty),
            .DispOrder = If(Not String.IsNullOrWhiteSpace(target.DispOrder), target.DispOrder, String.Empty),
            .ParentNo = If(Not String.IsNullOrWhiteSpace(target.ParentNo), target.ParentNo, String.Empty),
            .ExaminationN = target.ExaminationN.ConvertAll(Function(i) i.ToExaminationItem()),
            .ChildN = If(target.ChildN IsNot Nothing AndAlso target.ChildN.Any, target.ChildN.ConvertAll(Function(i) i.ToExaminationGroupItem()), Nothing)
       }

    End Function

#End Region

#Region "QhApiExaminationItem"

    ''' <summary>
    ''' 検査結果項目クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 検査結果項目クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToExaminationItem(target As QhApiExaminationItem) As ExaminationItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New ExaminationItem() With {
            .ItemType = target.ItemType.ToValueType(Of QyExaminationItemTypeEnum)(),
            .Code = target.Code,
            .Name = target.Name,
            .ValueType = target.ValueType.ToValueType(Of QyExaminationItemValueTypeEnum)(),
            .Value = target.Value,
            .Unit = target.Unit,
            .Interpretation = target.Interpretation,
            .Low = target.Low,
            .High = target.High,
            .ReferenceDisplayName = target.ReferenceDisplayName,
            .Comment = target.Comment,
            .DispOrder = target.DispOrder
        }

    End Function

#End Region


#Region "QhApiExaminationJudgementItem"

    ''' <summary>
    ''' 検査所見・判定情報クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 検査結果項目クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToExaminationJudgementItem(target As QhApiExaminationJudgementItem) As ExaminationJudgementItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New ExaminationJudgementItem() With {
            .CourseName = target.CourseName,
            .IsTotalJudgment = target.IsTotalJudgment.ToValueType(Of Boolean)(),
            .Judgment1 = target.Judgment1,
            .Judgment2 = target.Judgment2,
            .JudgmentContent1 = target.JudgmentContent1,
            .JudgmentContent2 = target.JudgmentContent2,
            .LocalCode = target.LocalCode,
            .Name = target.Name,
            .Value = target.Value
        }

    End Function

#End Region

#Region "QhApiExaminationAssociatedFileItem"


    ''' <summary>
    ''' 検査結果項目クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' 検査結果項目クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToExaminationAssociatedFileItem(target As QhApiExaminationAssociatedFileItem) As ExaminationAssociatedFileItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New ExaminationAssociatedFileItem() With {
            .AdditionalKey = target.AdditionalKey,
            .DataKey = Guid.Parse(target.DataKey),
            .DataType = Byte.Parse(target.DataType),
            .FacilityKey = Guid.Parse(target.FacilityKey),
            .LinkageSystemId = target.LinkageSystemId,
            .LinkageSystemNo = Integer.Parse(target.LinkageSystemNo),
            .RecordDate = Date.Parse(target.RecordDate)
        }

    End Function

#End Region

#Region "QjApiCityItem"

    ''' <summary>
    ''' API 用のクーポン情報クラスを、
    ''' 市町村 マスタ クラスへ変換します。
    ''' </summary>
    ''' <param name="target">変換元クラス。</param>
    ''' <returns>
    ''' クーポン情報クラス。
    ''' </returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function ToCityItem(target As QjApiCityItem) As CityItem

        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

        Return New CityItem() With {
            .AreaNo = Integer.Parse(target.AreaNo),
            .CityNo = target.CityNo,
            .CityName = target.CityName,
            .DispOrder = Integer.Parse(target.DispOrder),
            .InstitutionCount = Integer.Parse(target.InstitutionCount)
            }

    End Function

#End Region

    '#Region "QhApiAttachedFileItem"

    '    ''' <summary>
    '    ''' API 用の添付ファイル情報クラスを、
    '    ''' 添付ファイル情報クラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' 添付ファイル情報クラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToAttachedFileItem(target As QhApiAttachedFileItem) As AttachedFileItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Return New AttachedFileItem() With {
    '            .FileKey = Guid.Parse(target.FileKey),
    '            .TempFileKey = String.Empty,
    '            .OriginalName = target.OriginalName
    '        }

    '    End Function

    '#End Region

    '#Region "QhApiDisabilityItem"

    '    ''' <summary>
    '    ''' API 用の障害者手帳情報クラスを、
    '    ''' 障害者手帳情報クラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' 障害者手帳情報クラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToDisabilityItem(target As QhApiDisabilityItem) As DisabilityItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Return New DisabilityItem() With {
    '            .CertificateType = target.CertificateType.TryToValueType(QhDisabilityCertificateTypeEnum.None),
    '            .Name = target.Name,
    '            .Grade = target.Grade
    '        }

    '    End Function

    '#End Region

    '#Region "QhApiDiseaseItem"

    '    ''' <summary>
    '    ''' API 用の病気情報クラスを、
    '    ''' 病気情報クラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' 病気情報クラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToDiseaseItem(target As QhApiDiseaseItem) As DiseaseItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Return New DiseaseItem() With {
    '            .Name = target.Name,
    '            .FacilityName = target.FacilityName
    '        }

    '    End Function

    '#End Region

    '#Region "QhApiEmergencyContactItem"

    '    ''' <summary>
    '    ''' API 用の緊急連絡先情報クラスを、
    '    ''' 緊急連絡先情報クラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' 緊急連絡先情報クラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToEmergencyContactItem(target As QhApiEmergencyContactItem) As EmergencyContactItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Return New EmergencyContactItem() With {.ContactType = target.ContactType.TryToValueType(QhEmergencyContactTypeEnum.None),
    '            .Name = target.Name,
    '            .Relationship = target.Relationship,
    '            .FacilityName = target.FacilityName,
    '            .PhoneN = target.PhoneN.ConvertAll(Function(i) i.ToPhoneItem())
    '        }

    '    End Function

    '#End Region

    '#Region "QhApiLogItem"

    '    ' ''' <summary>
    '    ' ''' API 用の参照ログ情報クラスを、
    '    ' ''' 参照ログ情報クラスへ変換します。
    '    ' ''' </summary>
    '    ' ''' <param name="target">変換元クラス。</param>
    '    ' ''' <returns>
    '    ' ''' 参照ログ情報クラス。
    '    ' ''' </returns>
    '    ' ''' <remarks></remarks>
    '    '<Extension()>
    '    'Public Function ToLogItem(target As QhApiLogItem) As LogItem

    '    '    If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '    '    Return New LogItem() With {
    '    '        .ActionDate = Date.Parse(target.ActionDate),
    '    '        .ActionType = target.ActionType.TryToValueType(QhActionTypeEnum.None),
    '    '        .ActionStartDate = Date.Parse(target.ActionStartDate),
    '    '        .ActionEndDate = Date.Parse(target.ActionEndDate),
    '    '        .SystemType = target.SystemType.TryToValueType(QhSystemTypeEnum.None),
    '    '        .SystemName = target.SystemName,
    '    '        .ActorKey = Guid.Parse(target.ActorKey),
    '    '        .ActorName = target.ActorName,
    '    '        .ActorOrganizationName = target.ActorOrganizationName,
    '    '        .Comment = target.Comment
    '    '    }

    '    'End Function


    '    ''' <summary>
    '    ''' API 用の参照ログ情報クラスを、
    '    ''' 参照ログ情報クラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' 参照ログ情報クラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToLogItem(target As QhApiLogItem) As LogItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Return New LogItem() With {
    '            .ActionDate = Date.Parse(target.ActionDate),
    '            .ActionType = target.ActionType.TryToValueType(QhActionTypeEnum.None),
    '            .ActionStartDate = Date.Parse(target.ActionStartDate),
    '            .ActionEndDate = Date.Parse(target.ActionEndDate),
    '            .SystemType = target.SystemType.TryToValueType(QhSystemTypeEnum.None),
    '            .SystemName = target.SystemName,
    '            .ActorKey = Guid.Parse(target.ActorKey),
    '            .ActorOrganizationKey = Guid.Parse(target.ActorOrganizationKey),
    '            .Comment = target.Comment
    '        }

    '    End Function

    '#End Region

    '#Region "QhApiLogSetItem"

    '    ''' <summary>
    '    ''' API 用の日付ごとの参照ログ情報クラスを、
    '    ''' 日付ごとの参照ログ情報クラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' 日付ごとの参照ログ情報クラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToLogSetItem(target As QhApiLogSetItem) As LogSetItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Return New LogSetItem() With {
    '            .ActionDate = Date.Parse(target.ActionDate),
    '            .LogN = target.LogN.ConvertAll(Function(i) i.ToLogItem())
    '        }

    '    End Function

    '#End Region

    '#Region "QhApiPhoneItem"

    '    ''' <summary>
    '    ''' API 用の電話情報クラスを、
    '    ''' 電話情報クラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' 電話情報クラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToPhoneItem(target As QhApiPhoneItem) As PhoneItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Return New PhoneItem() With {
    '            .PhoneType = target.PhoneType.TryToValueType(QhPhoneTypeEnum.None),
    '            .PhoneNumber = target.PhoneNumber
    '        }

    '    End Function

    '#End Region

    '#Region "QhApiDentalItem"

    '    ''' <summary>
    '    ''' 歯の情報クラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' 歯の情報クラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToDentalItem(target As QhApiDentalItem) As DentalItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Dim fSet As New QhDentalFormulaSetItem()
    '        Dim sSet As New QhDentalStateSetItem()
    '        Dim ser As New QsJsonSerializer()
    '        Try
    '            fSet = ser.Deserialize(Of QhDentalFormulaSetItem)(target.DentalFormulaSet)
    '            sSet = ser.Deserialize(Of QhDentalStateSetItem)(target.DentalStateSet)
    '        Catch
    '        End Try

    '        Return New DentalItem() With {
    '            .RecordDate = Date.Parse(target.RecordDate),
    '            .Sequence = Integer.Parse(target.Sequence),
    '            .DataType = Byte.Parse(target.DataType),
    '            .DentalFormulaSet = fSet,
    '            .DentalStateSet = sSet
    '        }

    '    End Function

    '#End Region

    '#Region "QhApiPatientCardItem"

    '    ''' <summary>
    '    ''' 利用者カードクラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' 利用者カードクラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToPatientCardItem(target As QhApiPatientCardItem) As PatientCardItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Dim cardSet As New QhPatientCardSetItem()
    '        Dim ser As New QsJsonSerializer()
    '        Try
    '            cardSet = ser.Deserialize(Of QhPatientCardSetItem)(target.PatientCardSet)
    '        Catch
    '        End Try

    '        Return New PatientCardItem() With {
    '            .CardCode = Integer.Parse(target.CardCode),
    '            .Sequence = Integer.Parse(target.Sequence),
    '            .CreatedDate = Date.Parse(target.CreatedDate),
    '            .PatientCardSet = cardSet,
    '            .AttachedFileN = target.AttachedFileN.ConvertAll(Function(i) i.ToAttachedFileItem),
    '            .StatusType = Byte.Parse(target.StatusType)
    '        }

    '    End Function

    '#End Region

    '#Region "QhApiRelationItem"

    '    ''' <summary>
    '    ''' 開示情報クラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' 開示情報クラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToRelationItem(target As QhApiRelationItem) As RelationItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Return New RelationItem() With {
    '            .RelationFacilityKey = Guid.Parse(target.RelationFacilityKey),
    '            .StartDate = Date.Parse(target.StartDate),
    '            .EndDate = Date.Parse(target.EndDate),
    '            .FacilityName = target.FacilityName,
    '            .FacilityAddress = target.FacilityAddress,
    '            .FacilityTel = target.FacilityTel,
    '            .RelationOpenType = CType([Enum].Parse(GetType(QhRelationTypeEnum), target.RelationOpenType), QhRelationTypeEnum),
    '            .RelationShowType = CType([Enum].Parse(GetType(QhRelationTypeEnum), target.RelationShowType), QhRelationTypeEnum),
    '            .RelationEditType = CType([Enum].Parse(GetType(QhRelationTypeEnum), target.RelationEditType), QhRelationTypeEnum),
    '            .CreatedDate = Date.Parse(target.CreatedDate)
    '        }

    '    End Function

    '#End Region

    '#Region "QhApiFamilyAccountItem"

    '    ''' <summary>
    '    ''' 家族アカウントクラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' 家族アカウントクラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToFamilyAccountItem(target As QhApiFamilyAccountItem, targetAccountKey As Guid) As FamilyAccountItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Return New FamilyAccountItem() With {
    '            .AccountKey = Guid.Parse(target.AccountKey),
    '            .StartDate = Date.Parse(target.StartDate),
    '            .EndDate = Date.Parse(target.EndDate),
    '            .FamilyName = target.FamilyName,
    '            .MiddleName = target.MiddleName,
    '            .GivenName = target.GivenName,
    '            .FamilyKanaName = target.FamilyKanaName,
    '            .MiddleKanaName = target.MiddleKanaName,
    '            .GivenKanaName = target.GivenKanaName,
    '            .FamilyRomanName = target.FamilyRomanName,
    '            .MiddleRomanName = target.MiddleRomanName,
    '            .GivenRomanName = target.GivenRomanName,
    '            .SexType = target.SexType.ToValueType(Of QhSexTypeEnum)(),
    '            .Birthday = Date.Parse(target.Birthday),
    '            .PhotoKey = Guid.Parse(target.PhotoKey),
    '            .PrivateFlag = Boolean.Parse(target.PrivateFlag),
    '            .MarkFlag = False,
    '            .BrowsingFlag = If(.AccountKey = targetAccountKey, True, False),
    '            .EncryptedAccountKey = QhAccountItemBase.EncryptAccountKey(.AccountKey),
    '            .EncryptedPhotoReference = QhAccountItemBase.EncryptPhotoReference(.AccountKey, .PhotoKey, QhFileTypeEnum.Edited),
    '            .EncryptedThumbnailPhotoReference = QhAccountItemBase.EncryptPhotoReference(.AccountKey, .PhotoKey, QhFileTypeEnum.Thumbnail)
    '        }

    '    End Function

    '#End Region

    '#Region "QhApiVitalValueItem"

    '    ''' <summary>
    '    ''' API 用のバイタル情報クラスを、
    '    ''' バイタル情報クラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' バイタル情報クラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToVitalValueItem(target As QhApiVitalValueItem) As VitalValueItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Return New VitalValueItem() With {
    '            .RecordDate = Date.Parse(target.RecordDate),
    '            .VitalType = target.VitalType.TryToValueType(QhVitalTypeEnum.None),
    '            .Value1 = Decimal.Parse(target.Value1),
    '            .Value2 = Decimal.Parse(target.Value2),
    '            .ConditionType = target.ConditionType.TryToValueType(QhVitalConditionTypeEnum.None)
    '        }

    '    End Function


    '    ''' <summary>
    '    ''' API 用のバイタル情報クラスを、
    '    ''' バイタル情報インプットモデルへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' バイタル情報インプットモデル。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToVitalValueInputModel(target As QhApiVitalValueItem) As VitalValueInputModel

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Dim result As New VitalValueInputModel()
    '        Dim dateValue As Date = target.RecordDate.TryToValueType(Date.MinValue)
    '        Dim enumValue1 As QhVitalTypeEnum = target.VitalType.TryToValueType(QhVitalTypeEnum.None)
    '        Dim enumValue2 As QhVitalConditionTypeEnum = target.ConditionType.TryToValueType(QhVitalConditionTypeEnum.None)
    '        Dim decimalValue1 As Decimal = target.Value1.TryToValueType(Decimal.MinValue)
    '        Dim decimalValue2 As Decimal = target.Value2.TryToValueType(Decimal.MinValue)

    '        With target
    '            ' バイタル情報の種別
    '            result.VitalType = enumValue1

    '            ' AM/PM
    '            result.Meridiem = dateValue.ToString("tt", CultureInfo.InvariantCulture).ToLower()

    '            ' 時
    '            result.Hour = dateValue.ToString("%h")

    '            If result.Hour.CompareTo("12") = 0 Then result.Hour = "0"

    '            ' 分
    '            result.Minute = dateValue.ToString("%m")

    '            ' 測定値 1
    '            result.Value1 = If(decimalValue1 > 0, decimalValue1.ToString("0.####"), String.Empty)

    '            ' 測定値 2
    '            result.Value2 = If(decimalValue2 > 0, decimalValue2.ToString("0.####"), String.Empty)

    '            ' 測定タイミング
    '            result.ConditionType = enumValue2.ToString()

    '            result.IsEmpty = False
    '        End With

    '        Return result

    '    End Function

    '#End Region


    '#Region "QhApiNWPSLogItem"

    '    ''' <summary>
    '    ''' ネットプリント履歴クラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' ネットプリント履歴クラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToNWPSLogItem(target As QhApiNWPSLogItem) As NWPSLogItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Dim printTypeStr As String = String.Empty
    '        Select Case target.PrintType
    '            Case "1"
    '                printTypeStr = "健康情報サマリー"

    '        End Select

    '        Return New NWPSLogItem() With {
    '            .ActionDate = target.ActionDate,
    '            .PrintType = printTypeStr,
    '            .UserCode = target.UserCode,
    '            .DeleteDate = target.DeleteDate
    '        }

    '    End Function

    '#End Region

    '#Region "QhApiRequestItem"

    '    ''' <summary>
    '    ''' API 用のリクエストクラスを、
    '    ''' リクエストクラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' リクエストクラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToRequestItem(target As QhApiRequestItem) As RequestItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Return New RequestItem() With {
    '            .RequestAccountKey = Guid.Parse(target.RelationAccountKey),
    '            .RequestFacilityKey = Guid.Parse(target.RelationFacitilyKey),
    '            .Status = target.Status,
    '            .RequestName = target.RequestName,
    '            .CreatedDate = Date.Parse(target.CreatedDate),
    '            .PhotoKey = If(String.IsNullOrWhiteSpace(target.PhotoKey), Guid.Empty, Guid.Parse(target.PhotoKey))
    '        }

    '    End Function

    '#End Region


    '#Region "NoteVitalTargetValueInputModel"

    '    ''' <summary>
    '    ''' バイタル目標値情報インプットモデルを、
    '    ''' API 用バイタル目標値情報クラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元モデル。</param>
    '    ''' <returns>
    '    ''' API 用バイタル目標値情報。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToApiTargetValueItem(target As NoteVitalTargetValueInputModel) As QhApiTargetValueItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元モデルがNull参照です。")

    '        Return New QhApiTargetValueItem() With {
    '            .VitalType = target.VitalType,
    '            .ValueType = target.ValueType,
    '            .Lower = target.Lower,
    '            .Upper = target.Upper
    '        }
    '    End Function

    '#End Region

    '#Region "VitalValueInputModel"

    '    ''' <summary>
    '    ''' バイタル情報インプットモデルを、
    '    ''' API 用のバイタル情報クラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元モデル。</param>
    '    ''' <param name="recordDate">編集日。</param>
    '    ''' <returns>
    '    ''' API 用のバイタル情報クラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToApiVitalValueItem(target As VitalValueInputModel, recordDate As Date) As QhApiVitalValueItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元モデルがNull参照です。")

    '        Return New QhApiVitalValueItem() With {
    '            .RecordDate = Date.Parse(String.Format("#{0}/{1}/{2} {3}:{4}:00 {5}#", recordDate.Month, recordDate.Day, recordDate.Year, target.Hour, target.Minute, target.Meridiem)).ToApiDateString(),
    '            .VitalType = target.VitalType.ToString(),
    '            .Value1 = If(String.IsNullOrWhiteSpace(target.Value1), Decimal.MinValue, Decimal.Parse(target.Value1)).ToString(),
    '            .Value2 = If(String.IsNullOrWhiteSpace(target.Value2), Decimal.MinValue, Decimal.Parse(target.Value2)).ToString(),
    '            .ConditionType = If(String.IsNullOrWhiteSpace(target.ConditionType), QhVitalConditionTypeEnum.None, DirectCast([Enum].Parse(GetType(QhVitalConditionTypeEnum), target.ConditionType), QhVitalConditionTypeEnum)).ToString()
    '        }

    '    End Function

    '#End Region

    '#Region "AttachedFileItem"

    '    ''' <summary>
    '    ''' 添付ファイル情報クラスを、
    '    ''' API 用の添付ファイル情報クラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' API 用の添付ファイル情報クラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToApiAttachedFileItem(target As AttachedFileItem) As QhApiAttachedFileItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Return New QhApiAttachedFileItem() With {
    '            .FileKey = target.FileKey.ToApiGuidString(),
    '            .OriginalName = target.OriginalName
    '        }

    '    End Function

    '#End Region

    '#Region "PhoneItem"

    '    ''' <summary>
    '    ''' 電話情報クラスを、
    '    ''' API 用の電話情報クラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' API 用の電話情報クラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToApiPhoneItem(target As PhoneItem) As QhApiPhoneItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Return New QhApiPhoneItem() With {
    '            .PhoneType = target.PhoneType.ToString(),
    '            .PhoneNumber = target.PhoneNumber
    '        }

    '    End Function

    '#End Region

    '#Region "DisabilityItem"

    '    ''' <summary>
    '    ''' 障害者手帳情報クラスを、
    '    ''' API 用の障害者手帳情報クラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' API 用の障害者手帳情報クラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToApiToDisabilityItem(target As DisabilityItem) As QhApiDisabilityItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Return New QhApiDisabilityItem() With {
    '            .CertificateType = target.CertificateType.ToString(),
    '            .Name = target.Name,
    '            .Grade = target.Grade
    '        }

    '    End Function

    '#End Region

    '#Region "DiseaseItem"

    '    ''' <summary>
    '    ''' 病気情報クラスを、
    '    ''' API 用の病気情報クラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' API 用の病気情報クラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToApiDiseaseItem(target As DiseaseItem) As QhApiDiseaseItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Return New QhApiDiseaseItem() With {
    '            .Name = target.Name,
    '            .FacilityName = target.FacilityName
    '        }

    '    End Function

    '#End Region

    '#Region "EmergencyContactItem"

    '    ''' <summary>
    '    ''' 緊急連絡先情報クラスを、
    '    ''' API 用の緊急連絡先情報クラスへ変換します。
    '    ''' </summary>
    '    ''' <param name="target">変換元クラス。</param>
    '    ''' <returns>
    '    ''' API 用の緊急連絡先情報クラス。
    '    ''' </returns>
    '    ''' <remarks></remarks>
    '    <Extension()>
    '    Public Function ToApiEmergencyContactItem(target As EmergencyContactItem) As QhApiEmergencyContactItem

    '        If target Is Nothing Then Throw New ArgumentNullException("target", "変換元クラスがNull参照です。")

    '        Return New QhApiEmergencyContactItem() With {
    '            .ContactType = target.ContactType.ToString(),
    '            .Name = target.Name,
    '            .Relationship = target.Relationship,
    '            .FacilityName = target.FacilityName,
    '            .PhoneN = target.PhoneN.ConvertAll(Function(i) i.ToApiPhoneItem())
    '        }

    '    End Function

    '#End Region

End Module