''' <summary>
''' 「コルムス ヤプリ サイト」で使用する画面番号の種別を表します。
''' </summary>
''' <remarks>
''' 既存のメンバーを変更しないでください。
''' 必要に応じて新規の値を持つメンバーを追加してください。
''' </remarks>
Public Enum QyPageNoTypeEnum As Byte

    ''' <summary>
    ''' 未指定です。
    ''' </summary>
    ''' <remarks></remarks>
    None = 0

    ''' <summary>
    ''' 「ホーム」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    PortalHome = 1

    ''' <summary>
    ''' 「歩く」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    NoteWalk = 2

    ''' <summary>
    ''' 「運動」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    NoteExercise = 3

    ''' <summary>
    ''' 「食事」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    NoteMeal = 4

    ''' <summary>
    ''' 「バイタル」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    NoteVital = 5

    ''' <summary>
    ''' 「健康年齢」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    HealthAge = 6

    ''' <summary>
    ''' 「健康年齢更新」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    HealthAgeEdit = 7

    ''' <summary>
    ''' 「医療機関検索」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    PortalSearch = 8

    ''' <summary>
    ''' 「医療機関検索詳細」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    PortalSearchDetail = 9

    ''' <summary>
    ''' 「初期設定」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    StartSetup = 10

    ''' <summary>
    ''' 「情報」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete("検討中")>
    PortalInformation = 11

    ''' <summary>
    ''' 「基本設定」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete("検討中")>
    PortalBasicSetting = 12

    ''' <summary>
    ''' 「目標設定」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete("検討中")>
    PortalTargetSetting = 13

    ''' <summary>
    ''' 「利用規約」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete("検討中")>
    PortalTerms = 14

    ''' <summary>
    ''' 「退会」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete("検討中")>
    PortalUnsubscribe = 15

    ''' <summary>
    ''' 「プレミアム」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    PremiumIndex = 16

    ''' <summary>
    ''' 「プレミアム登録課金選択」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    PremiumRegist = 17

    ''' <summary>
    ''' 「お支払い履歴」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    PremiumHistory = 18

    ''' <summary>
    ''' 「JOTOポイント履歴」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete("検討中")>
    PortalHistory = 19

    ''' <summary>
    ''' 「gulfスポーツ動画」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    <Obsolete("検討中")>
    NoteGulfMovie = 20

    ''' <summary>
    ''' 「法人連携」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    PortalCompanyConnection = 21

    ''' <summary>
    ''' 「病院連携」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    PortalHospitalConnection = 22

    ''' <summary>
    ''' 「健診結果」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    NoteExamination = 23

    ''' <summary>
    ''' 「連携設定」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    PortalConnectionSetting = 24

    ''' <summary>
    ''' 「チャレンジ」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    PortalChallenge = 25

    ''' <summary>
    ''' 「レシピ動画」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    NoteRecipeMovie = 26

    ''' <summary>
    ''' 「おくすり連携」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    PortalMedicineConnection = 29

    ''' <summary>
    ''' 「おくすり」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    NoteMedicine = 30

    ''' <summary>
    ''' 「通いの場連携」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    PortalKayoinoba = 31

    ''' <summary>
    ''' 「心拍」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    NoteHeartRate = 32

    ''' <summary>
    ''' 「METs」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    NoteMets = 33

   ''' <summary>
    ''' 「法人連携ホームメニュー」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    PortalCompanyConnectionHome = 34

    ''' <summary>
    ''' 「Fitbitクーポン交換」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    PortalCouponForFitbit = 35

    ''' <summary>
    ''' 「SMSパスワードリセット」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    PasswordResetResetRecoverSms = 36

    ''' <summary>
    ''' 「PremiumPayJp」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    PremiumPayJpRegist = 37

    ''' <summary>
    ''' 「PremiumPayJp」画面です。
    ''' </summary>
    ''' <remarks></remarks>
    PremiumPayJpEdit = 38


End Enum
