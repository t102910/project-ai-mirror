Imports MGF.QOLMS.QolmsApiCoreV1

''' <summary>
''' QOLMSポイント項目を表します。
''' </summary>
''' <remarks>
''' 既存のメンバーを変更しないでください。
''' 必要に応じて新規の値を持つメンバーを追加してください。
''' </remarks>
Public Enum QyPointItemTypeEnum As Byte
    ''' <summary>
    ''' 未指定です。
    ''' </summary>
    ''' <remarks></remarks>
    None = 0
    ''' <summary>
    ''' 初回プレミアム登録ポイントです。
    ''' </summary>
    ''' <remarks></remarks>
    InitialRegistration = 1
    ''' <summary>
    ''' ログインポイントです。
    ''' </summary>
    ''' <remarks></remarks>
    Login = 2
    ''' <summary>
    ''' 歩数のポイントです。
    ''' </summary>
    ''' <remarks></remarks>
    Walk5k = 3
    Walk6k = 4
    Walk7k = 5
    Walk8k = 6
    Walk9k = 7
    Walk10k = 8
    ''' <summary>
    ''' 運動のポイントです。
    ''' </summary>
    ''' <remarks></remarks>
    Exercise = 9
    ''' <summary>
    ''' 朝食登録のポイントです。
    ''' </summary>
    ''' <remarks></remarks>
    Breakfast = 10
    ''' <summary>
    ''' 昼食登録のポイントです。
    ''' </summary>
    ''' <remarks></remarks>
    Lunch = 11
    ''' <summary>
    ''' 夕食登録のポイントです。
    ''' </summary>
    ''' <remarks></remarks>
    Dinner = 12
    ''' <summary>
    ''' 間食登録のポイントです。
    ''' </summary>
    ''' <remarks></remarks>
    Snack = 13
    ''' <summary>
    ''' バイタル登録のポイントです。
    ''' </summary>
    ''' <remarks></remarks>
    Vital = 14
    ''' <summary>
    ''' 健診データ登録のポイントです。
    ''' </summary>
    ''' <remarks></remarks>
    Examination = 15

    ''' <summary>
    ''' データチャージ登録のポイントです。
    ''' </summary>
    ''' <remarks></remarks>
    Datacharge = 16

    ''' <summary>
    ''' ポイント交換登録のポイントです。
    ''' </summary>
    ''' <remarks></remarks>
    PointExchange = 17

    ''' <summary>
    ''' タニタ連携初回登録のポイントです。
    ''' </summary>
    ''' <remarks></remarks>
    TanitaConnection = 18

    ''' <summary>
    ''' auポイント交換登録のポイントです。
    ''' </summary>
    ''' <remarks></remarks>
    AuPoint = 19

    ''' <summary>
    ''' auポイント交換登録のポイントです。
    ''' </summary>
    ''' <remarks></remarks>
    AmazonPoint = 20

    ''' <summary>
    ''' ポイント修正登録のポイントです。
    ''' </summary>
    ''' <remarks></remarks>
    RecoveryPoint = 21

    ''' <summary>
    ''' 健診結果登録のポイントです。
    ''' </summary>
    ''' <remarks></remarks>
    ExaminationPoint = 22

    ''' <summary>
    ''' チャレンジ達成ポイントのポイントです。
    ''' </summary>
    ''' <remarks></remarks>
    ChallengeCompleted = 23

    ''' <summary>
    ''' 食事登録の ポイント です（2022/02/28 からの仕様）
    ''' </summary>
    ''' <remarks></remarks>
    Meal = 24

End Enum
