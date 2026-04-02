using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using MGF.QOLMS.QolmsApiCoreV1;
using MGF.QOLMS.QolmsApiEntityV1;
using MGF.QOLMS.QolmsDbEntityV1;

namespace MGF.QOLMS.QolmsJotoWebView
{
    /// <summary>
    /// 「沖縄セルラー Yappli」で使用する API 用エンティティクラスを、
    /// 異なる形式のエンティティクラスへ変換するための拡張機能を提供します。
    /// </summary>
    internal static class QjApiEntityConverterExtension
    {
        //#region "QhApiExerciseItem"

        ///// <summary>
        ///// API 用の運動の情報クラスを、
        ///// 運動の情報クラスへ変換します。
        ///// </summary>
        ///// <param name="target">変換元クラス。</param>
        ///// <returns>
        ///// 運動の情報クラス。
        ///// </returns>
        //public static ExerciseItem ToExerciseItem(this QhApiExerciseItem target)
        //{
        //    if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

        //    return new ExerciseItem
        //    {
        //        ExerciseDate = DateTime.Parse(target.ExerciseDate),
        //        ExerciseType = target.ExerciseType,
        //        Sequence = int.Parse(target.Sequence),
        //        ExerciseName = target.ExerciseName,
        //        Calorie = short.Parse(target.Calorie),
        //        PhotoKey = target.PhotoKey
        //    };
        //}

        //#endregion

        //#region "QhApiMealItem"

        ///// <summary>
        ///// API 用の食事の情報クラスを、
        ///// 食事の情報クラスへ変換します。
        ///// </summary>
        ///// <param name="target">変換元クラス。</param>
        ///// <returns>
        ///// 食事の情報クラス。
        ///// </returns>
        //public static MealItem ToMealItem(this QhApiMealItem target)
        //{
        //    if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

        //    return new MealItem
        //    {
        //        MealDate = DateTime.Parse(target.MealDate),
        //        MealType = target.MealType,
        //        Sequence = int.Parse(target.Sequence),
        //        MealName = target.MealName,
        //        Calorie = short.Parse(target.Calorie),
        //        PhotoKey = Guid.Parse(target.PhotoKey),
        //        FoodN = target.FoodN.ToList().ConvertAll(i => new FoodItem
        //        {
        //            label = i.label,
        //            probability = i.probability,
        //            calorie = i.calorie,
        //            protein = i.protein,
        //            lipid = i.lipid,
        //            carbohydrate = i.carbohydrate,
        //            salt_amount = i.salt_amount,
        //            available_carbohydrate = i.available_carbohydrate,
        //            fiber = i.fiber
        //        })
        //    };
        //}

        //#endregion

        //#region "QhApiVitalValueItem"

        ///// <summary>
        ///// API 用のバイタル情報クラスを、
        ///// バイタル情報クラスへ変換します。
        ///// </summary>
        ///// <param name="target">変換元クラス。</param>
        ///// <returns>
        ///// バイタル情報クラス。
        ///// </returns>
        //public static VitalValueItem ToVitalValueItem(this QhApiVitalValueItem target)
        //{
        //    if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

        //    return new VitalValueItem
        //    {
        //        RecordDate = DateTime.Parse(target.RecordDate),
        //        VitalType = target.VitalType.TryToValueType(QjVitalTypeEnum.None),
        //        Value1 = decimal.Parse(target.Value1),
        //        Value2 = decimal.Parse(target.Value2),
        //        ConditionType = target.ConditionType.TryToValueType(QjVitalConditionTypeEnum.None)
        //    };
        //}

        //#endregion

        //#region "VitalValueInputModel"

        ///// <summary>
        ///// バイタル情報インプットモデルを、
        ///// API 用のバイタル情報クラスへ変換します。
        ///// </summary>
        ///// <param name="target">変換元モデル。</param>
        ///// <param name="recordDate">編集日。</param>
        ///// <returns>
        ///// API 用のバイタル情報クラス。
        ///// </returns>
        //public static QhApiVitalValueItem ToApiVitalValueItem(this VitalValueInputModel target, DateTime recordDate)
        //{
        //    if (target == null) throw new ArgumentNullException("target", "変換元モデルがNull参照です。");

        //    return new QhApiVitalValueItem
        //    {
        //        RecordDate = DateTime.Parse(string.Format(
        //                "#{0}/{1}/{2} {3}:{4}:00 {5}#",
        //                recordDate.Month,
        //                recordDate.Day,
        //                recordDate.Year,
        //                target.Hour,
        //                target.Minute,
        //                target.Meridiem
        //            ))
        //            .ToApiDateString(),
        //        VitalType = target.VitalType.ToString(),
        //        Value1 = (string.IsNullOrWhiteSpace(target.Value1) ? decimal.MinValue : decimal.Parse(target.Value1)).ToString(),
        //        Value2 = (string.IsNullOrWhiteSpace(target.Value2) ? decimal.MinValue : decimal.Parse(target.Value2)).ToString(),
        //        ConditionType = (string.IsNullOrWhiteSpace(target.ConditionType)
        //                ? QjVitalConditionTypeEnum.None
        //                : (QjVitalConditionTypeEnum)Enum.Parse(typeof(QjVitalConditionTypeEnum), target.ConditionType))
        //            .ToString()
        //    };
        //}

        //#endregion

        #region "QhApiHealthAgeValueItem"

        public static HealthAgeValueItem ToHealthAgeValueItem(this QhApiHealthAgeValueItem target)
        {
            if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

            return new HealthAgeValueItem
            {
                RecordDate = DateTime.Parse(target.RecordDate),
                HealthAgeValueType = target.HealthAgeValueType.TryToValueType(QjHealthAgeValueTypeEnum.None),
                Value = decimal.Parse(target.Value),
                Comparison = decimal.Parse(target.Comparison),
                IsRedCode = target.IsRedCode.TryToValueType(false),
                Label = target.Label,
                SortOrder = int.Parse(target.SortOrder)
            };
        }

        #endregion

        #region "QhApiHealthAgeReportItem"

        public static HealthAgeReportItem ToHealthAgeReportItem(this QhApiHealthAgeReportItem target)
        {
            if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

            return new HealthAgeReportItem
            {
                HealthAgeReportType = target.HealthAgeReportType.TryToValueType(QjHealthAgeReportTypeEnum.None),
                HealthAgeValueN = target.HealthAgeValueN.ConvertAll(i => i.ToHealthAgeValueItem()),
                Deviance = decimal.Parse(target.Deviance)
            };
        }

        #endregion

        #region "QhApiHealthAgeAdviceGoalItem"

        public static HealthAgeAdviceGoalItem ToHealthAgeAdviceGoalItem(this QhApiHealthAgeAdviceGoalItem target)
        {
            if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

            return new HealthAgeAdviceGoalItem
            {
                Name = target.Name,
                Old = decimal.Parse(target.Old),
                Goal = decimal.Parse(target.Goal)
            };
        }

        #endregion

        #region "QhApiHealthAgeAdviceItem"

        public static HealthAgeAdviceItem ToHealthAgeAdviceItem(this QhApiHealthAgeAdviceItem target)
        {
            if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

            return new HealthAgeAdviceItem
            {
                Title = target.Title,
                Based = target.Based,
                DetailN = target.DetailN,
                GoalN = target.GoalN.ConvertAll(i => i.ToHealthAgeAdviceGoalItem())
            };
        }

        #endregion

        //#region "QhApiMedicalInstitutionItem"

        ///// <summary>
        ///// API 用の医療機関情報クラスを、
        ///// 医療機関情報クラスへ変換します。
        ///// </summary>
        ///// <param name="target">変換元クラス。</param>
        ///// <returns>
        ///// 医療機関情報クラス。
        ///// </returns>
        //public static MedicalInstitutionItem ToMedicalInstitutionItem(this QhApiMedicalInstitutionItem target)
        //{
        //    if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

        //    return new MedicalInstitutionItem
        //    {
        //        CodeNo = int.Parse(target.CodeNo),
        //        InstitutionName = target.InstitutionName,
        //        KanaName = target.KanaName,
        //        PostalCode = target.PostalCode,
        //        Address = target.Address,
        //        DepartmentN = target.DepartmentN,
        //        OptionFlags = int.Parse(target.OptionFlags)
        //    };
        //}

        //#endregion

        //#region "QhApiMedicalInstitutionAcceptedItem"

        ///// <summary>
        ///// API 用の医療機関診療時間クラスを、
        ///// 医療機関情報クラスへ変換します。
        ///// </summary>
        ///// <param name="target">変換元クラス。</param>
        ///// <returns>
        ///// 医療機関情報クラス。
        ///// </returns>
        //public static MedicalInstitutionAcceptedItem ToMedicalInstitutionAcceptedItem(this QhApiMedicalInstitutionAcceptedItem target)
        //{
        //    if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

        //    return new MedicalInstitutionAcceptedItem
        //    {
        //        AccepteDays = target.AccepteDays.ConvertAll(i => bool.Parse(i)),
        //        AcceptedStart = target.AcceptedStart,
        //        AcceptedEnd = target.AcceptedEnd
        //    };
        //}

        //#endregion

        //#region "QhApiMedicalOfficeHouers"

        ///// <summary>
        ///// API 用の医療機関診療時間クラスを、
        ///// 医療機関情報クラスへ変換します。
        ///// </summary>
        ///// <param name="target">変換元クラス。</param>
        ///// <returns>
        ///// 医療機関情報クラス。
        ///// </returns>
        //public static MedicalOfficeHouers ToMedicalOfficeHouers(this QhApiMedicalOfficeHouers target)
        //{
        //    if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

        //    return new MedicalOfficeHouers
        //    {
        //        CodeNo = int.Parse(target.CodeNo),
        //        DayOfWeek = int.Parse(target.DayOfWeek),
        //        AcceptedTimeNo = int.Parse(target.AcceptedTimeNo),
        //        AcceptedStart = target.AcceptedStart,
        //        AcceptedEnd = target.AcceptedEnd
        //    };
        //}

        //#endregion

        //#region "QhApiPaymentLogItem"

        ///// <summary>
        ///// API 用の支払い（定期課金）ログ情報クラスを、
        ///// 支払い（定期課金）ログ情報クラスへ変換します。
        ///// </summary>
        ///// <param name="target">変換元クラス。</param>
        ///// <returns>
        ///// 支払い（定期課金）ログ情報クラス。
        ///// </returns>
        //public static PaymentLogItem ToPaymentLogItem(this QhApiPaymentLogItem target)
        //{
        //    if (target == null) throw new ArgumentNullException("target", "変換元クラスが Null 参照です。");

        //    return new PaymentLogItem
        //    {
        //        PaymentYear = int.Parse(target.PaymentYear),
        //        PaymentMonth = int.Parse(target.PaymentMonth),
        //        PaymentDate = DateTime.Parse(target.PaymentDate),
        //        PaymentType = byte.Parse(target.PaymentType),
        //        Amount = decimal.Parse(target.Amount),
        //        StatusCode = target.StatusCode
        //    };
        //}

        //#endregion

        //#region "QhApiDatachargeEventIdItem"

        ///// <summary>
        ///// API 用のデータチャージイベントIDクラスを、
        ///// データチャージイベントIDクラスへ変換します。
        ///// </summary>
        ///// <param name="target">変換元クラス。</param>
        ///// <returns>
        ///// 医療機関情報クラス。
        ///// </returns>
        //public static DatachargeEventIdItem ToDatachargeEventIdItem(this QhApiDatachargeEventIdItem target)
        //{
        //    if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

        //    return new DatachargeEventIdItem
        //    {
        //        EventId = target.EventId,
        //        Size = int.Parse(target.Size),
        //        Point = int.Parse(target.Point),
        //        DispName = target.DispName
        //    };
        //}

        //#endregion

        //#region "QhApiDatachargeHistItem"

        ///// <summary>
        ///// API 用のデータチャージ履歴クラスを、
        ///// データチャージ履歴クラスへ変換します。
        ///// </summary>
        ///// <param name="target">変換元クラス。</param>
        ///// <returns>
        ///// 医療機関情報クラス。
        ///// </returns>
        //public static DatachargeHistItem ToDatachargeHistItem(this QhApiDatachargeHistItem target)
        //{
        //    if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

        //    return new DatachargeHistItem
        //    {
        //        ActionDate = DateTime.Parse(target.ActionDate),
        //        Size = int.Parse(target.Size),
        //        Point = int.Parse(target.Point),
        //        DispName = target.DispName
        //    };
        //}

        //#endregion

        //#region "QhApiCouponItem"

        ///// <summary>
        ///// API 用のクーポン情報クラスを、
        ///// ポイント交換履歴クラスへ変換します。
        ///// </summary>
        ///// <param name="target">変換元クラス。</param>
        ///// <returns>
        ///// 医療機関情報クラス。
        ///// </returns>
        //public static CouponItem ToCouponItem(this QhApiCouponItem target)
        //{
        //    if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

        //    return new CouponItem
        //    {
        //        CouponType = byte.Parse(target.CouponType),
        //        Point = int.Parse(target.Point),
        //        DispName = target.DispName
        //    };
        //}

        //#endregion

        //#region "QhApiPointExchangeHistItem"

        ///// <summary>
        ///// API 用のポイント交換履歴クラスを、
        ///// ポイント交換履歴クラスへ変換します。
        ///// </summary>
        ///// <param name="target">変換元クラス。</param>
        ///// <returns>
        ///// 医療機関情報クラス。
        ///// </returns>
        //public static PointExchangeHistItem ToPointExchangeHistItem(this QhApiPointExchangeHistItem target)
        //{
        //    if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

        //    return new PointExchangeHistItem
        //    {
        //        IssueDate = DateTime.Parse(target.IssueDate),
        //        ExpirationDate = DateTime.Parse(target.ExpirationDate),
        //        CouponType = byte.Parse(target.CouponType),
        //        CouponId = target.CouponId,
        //        DispName = target.DispName,
        //        Point = int.Parse(target.Point)
        //    };
        //}

        //#endregion

        //#region "QhApiSearchMstItem"

        ///// <summary>
        ///// API 用の医療機関情報クラスを、
        ///// 医療機関情報クラスへ変換します。
        ///// </summary>
        ///// <param name="target">変換元クラス。</param>
        ///// <returns>
        ///// 医療機関情報クラス。
        ///// </returns>
        //public static SearchMstItem ToSearchMstItem(this QhApiSearchMstItem target)
        //{
        //    if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

        //    return new SearchMstItem
        //    {
        //        Key = target.Key,
        //        SubKey = target.SubKey,
        //        Value = target.Value,
        //        SubValue = target.SubValue
        //    };
        //}

        //#endregion

        //#region "QhApiAuWalletPointItem"

        ///// <summary>
        ///// API 用のau WALLET ポイントクラスを、
        ///// au WALLET ポイントクラスへ変換します。
        ///// </summary>
        ///// <param name="target">変換元クラス。</param>
        ///// <returns>
        ///// 医療機関情報クラス。
        ///// </returns>
        //public static AuWalletPointItem ToAuWalletPointItem(this QhApiAuWalletPointItem target)
        //{
        //    if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

        //    return new AuWalletPointItem
        //    {
        //        AuWalletPointItemId = target.AuWalletPointItemId,
        //        Point = int.Parse(target.Point),
        //        DispName = target.DispName
        //    };
        //}

        //#endregion

        //#region "QhApiAuWalletPointItem"

        ///// <summary>
        ///// API 用のau WALLET ポイント交換履歴クラスを、
        ///// au WALLET ポイント交換履歴クラスへ変換します。
        ///// </summary>
        ///// <param name="target">変換元クラス。</param>
        ///// <returns>
        ///// 医療機関情報クラス。
        ///// </returns>
        //public static AuWalletPointHistItem ToAuWalletPointHistItem(this QhApiAuWalletPointHistItem target)
        //{
        //    if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

        //    return new AuWalletPointHistItem
        //    {
        //        ActionDate = DateTime.Parse(target.ActionDate),
        //        Point = int.Parse(target.Point),
        //        DispName = target.DispName
        //    };
        //}

        //#endregion

        //#region "QhApiAmazonGiftCardItem"

        ///// <summary>
        ///// API 用のAmazonギフト券情報クラスを、
        ///// Amazonギフト券クラスへ変換します。
        ///// </summary>
        ///// <param name="target">変換元クラス。</param>
        ///// <returns>
        ///// 医療機関情報クラス。
        ///// </returns>
        //public static AmazonGiftCardItem ToAmazonGiftCardItem(this QhApiAmazonGiftCardItem target)
        //{
        //    if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

        //    return new AmazonGiftCardItem
        //    {
        //        GiftCardType = byte.Parse(target.GiftcardType),
        //        DemandPoint = int.Parse(target.DemandPoint),
        //        GiftCardName = target.GiftcardName
        //    };
        //}

        //#endregion

        //#region "QhApiAmazonGiftCardHistItem"

        ///// <summary>
        ///// API 用のAmazonギフト券交換履歴クラスを、
        ///// Amazonギフト券交換履歴クラスへ変換します。
        ///// </summary>
        ///// <param name="target">変換元クラス。</param>
        ///// <returns>
        ///// 医療機関情報クラス。
        ///// </returns>
        //public static AmazonGiftCardHistItem ToAmazonGiftCardHistItem(this QhApiAmazonGiftCardHistItem target)
        //{
        //    if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

        //    return new AmazonGiftCardHistItem
        //    {
        //        IssueDate = DateTime.Parse(target.IssueDate),
        //        ExpirationDate = DateTime.Parse(target.ExpirationDate),
        //        GiftCardType = byte.Parse(target.GiftcardType),
        //        GiftCardId = target.GiftcardId,
        //        GiftCardName = target.GiftcardName,
        //        DemandPoint = int.Parse(target.DemandPoint)
        //    };
        //}

        //#endregion

        #region "QhApiExaminationSetItem"

        /// <summary>
        /// API 用の日付け、連番ごとの検査情報クラスを、
        /// 日付け、連番ごとの検査情報クラスへ変換します。
        /// </summary>
        /// <param name="target">変換元クラス。</param>
        /// <returns>
        /// 検査情報クラス。
        /// </returns>
        public static ExaminationSetItem ToExaminationSetItem(this QhApiExaminationSetItem target)
        {
            if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

            return new ExaminationSetItem
            {
                CacheKey = target.RecordDate + target.Sequence,
                CategoryId = target.CategoryId.ToValueType<int>(),
                RecordDate = target.RecordDate.ToValueType<DateTime>(),
                OrganizationKey = target.OrganizationKey,
                OrganizationName = target.OrganizationName,
                OrganizationTel = target.OrganizationTel,
                ExaminationN = target.ExaminationN.ConvertAll(i => i.ToExaminationItem()),
                HealthAge = target.HealthAge.TryToValueType(int.MinValue),
                ExaminationJudgementN = target.ExaminationJudgementN.ConvertAll(i => i.ToExaminationJudgementItem()),
                OverallAssessmentPdfN = target.OverallAssessmentPdfN.ConvertAll(i => i.ToExaminationAssociatedFileItem())
            };
        }

        #endregion

        #region "QhApiExaminationGroupItem"

        /// <summary>
        /// API 用の日付け、連番ごとの検査グループ情報クラスを、
        /// 日付け、連番ごとの検査情報クラスへ変換します。
        /// </summary>
        /// <param name="target">変換元クラス。</param>
        /// <returns>
        /// 検査グループ情報クラス。
        /// </returns>
        public static ExaminationGroupItem ToExaminationGroupItem(this QhApiExaminationGroupItem target)
        {
            if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

            return new ExaminationGroupItem
            {
                GroupNo = target.GroupNo.ToValueType<int>(),
                Name = target.Name,
                Comment = !string.IsNullOrWhiteSpace(target.Comment) ? target.Comment : string.Empty,
                DispOrder = !string.IsNullOrWhiteSpace(target.DispOrder) ? target.DispOrder : string.Empty,
                ParentNo = !string.IsNullOrWhiteSpace(target.ParentNo) ? target.ParentNo : string.Empty,
                ExaminationN = target.ExaminationN.ConvertAll(i => i.ToExaminationItem()),
                ChildN = (target.ChildN != null && target.ChildN.Any())
                    ? target.ChildN.ConvertAll(i => i.ToExaminationGroupItem())
                    : null
            };
        }

        #endregion

        #region "QhApiExaminationItem"

        /// <summary>
        /// 検査結果項目クラスへ変換します。
        /// </summary>
        /// <param name="target">変換元クラス。</param>
        /// <returns>
        /// 検査結果項目クラス。
        /// </returns>
        public static ExaminationItem ToExaminationItem(this QhApiExaminationItem target)
        {
            if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

            return new ExaminationItem
            {
                ItemType = target.ItemType.ToValueType<QjExaminationItemTypeEnum>(),
                Code = target.Code,
                Name = target.Name,
                ValueType = target.ValueType.ToValueType<QjExaminationItemValueTypeEnum>(),
                Value = target.Value,
                Unit = target.Unit,
                Interpretation = target.Interpretation,
                Low = target.Low,
                High = target.High,
                ReferenceDisplayName = target.ReferenceDisplayName,
                Comment = target.Comment,
                DispOrder = target.DispOrder
            };
        }

        #endregion

        #region "QhApiExaminationJudgementItem"

        /// <summary>
        /// 検査所見・判定情報クラスへ変換します。
        /// </summary>
        /// <param name="target">変換元クラス。</param>
        /// <returns>
        /// 検査結果項目クラス。
        /// </returns>
        public static ExaminationJudgementItem ToExaminationJudgementItem(this QhApiExaminationJudgementItem target)
        {
            if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

            return new ExaminationJudgementItem
            {
                CourseName = target.CourseName,
                IsTotalJudgment = target.IsTotalJudgment.ToValueType<bool>(),
                Judgment1 = target.Judgment1,
                Judgment2 = target.Judgment2,
                JudgmentContent1 = target.JudgmentContent1,
                JudgmentContent2 = target.JudgmentContent2,
                LocalCode = target.LocalCode,
                Name = target.Name,
                Value = target.Value
            };
        }

        #endregion

        #region "QhApiExaminationAssociatedFileItem"

        /// <summary>
        /// 検査結果項目クラスへ変換します。
        /// </summary>
        /// <param name="target">変換元クラス。</param>
        /// <returns>
        /// 検査結果項目クラス。
        /// </returns>
        public static ExaminationAssociatedFileItem ToExaminationAssociatedFileItem(this QhApiExaminationAssociatedFileItem target)
        {
            if (target == null) throw new ArgumentNullException("target", "変換元クラスがNull参照です。");

            return new ExaminationAssociatedFileItem
            {
                AdditionalKey = target.AdditionalKey,
                DataKey = Guid.Parse(target.DataKey),
                DataType = byte.Parse(target.DataType),
                FacilityKey = Guid.Parse(target.FacilityKey),
                LinkageSystemId = target.LinkageSystemId,
                LinkageSystemNo = int.Parse(target.LinkageSystemNo),
                RecordDate = DateTime.Parse(target.RecordDate)
            };
        }

        #endregion
    }
}
