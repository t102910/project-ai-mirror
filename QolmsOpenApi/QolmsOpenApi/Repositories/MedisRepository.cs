using MGF.QOLMS.QolmsDbCoreV1;
using MGF.QOLMS.QolmsDbEntityV1;
using MGF.QOLMS.QolmsOpenApi.Sql;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Repositories
{
    /// <summary>
    /// MEDIS 標準病名マスタ関連の入出力インターフェース
    /// </summary>
    public interface IMedisRepository
    {
        /// <summary>
        /// 形態素を元に形態素辞書マッピングマスタからスコアを計算したデータを取得する
        /// </summary>
        /// <param name="source">ソース文字列</param>
        /// <param name="morpheme">形態素</param>
        /// <param name="morphmeKana">形態素カナ</param>
        /// <param name="maxResults">取得件数</param>
        /// <returns>QH_MEDISMORPHEME_SCORE_VIEWのリスト</returns>
        List<QH_MEDISMORPHEME_SCORE_VIEW> ReadMorphemeScoreList(string source, string morpheme, string morphmeKana, int maxResults = 10);

        /// <summary>
        /// 形態素スコア情報を元に標準病名マスタ情報を取得する
        /// </summary>
        /// <param name="morphemeScoreList">形態素スコア情報のリスト</param>
        /// <returns></returns>
        List<QH_MEDISDISEASE_MST> ReadDiseaseListByScoreView(List<QH_MEDISMORPHEME_SCORE_VIEW> morphemeScoreList);
    }

    /// <summary>
    /// MEDIS 標準病名マスタ関連の入出力実装
    /// <inheritdoc cref="IMedisRepository"/>
    /// </summary>
    public class MedisRepository:QsDbReaderBase, IMedisRepository
    {       
        /// <inheritdoc/>
        public List<QH_MEDISMORPHEME_SCORE_VIEW> ReadMorphemeScoreList(string source, string morpheme, string morphmeKana,int maxResults = 10)
        {
            using (var con = QsDbManager.CreateDbConnection<QH_MEDISMORPHEME_MST>())
            {
                var paramList = new List<DbParameter>() {
                    CreateParameter(con, "@maxResults", maxResults),
                    CreateParameter(con, "@source", source ?? string.Empty),
                    CreateParameter(con, "@morpheme", morpheme ?? string.Empty),
                    CreateParameter(con, "@morphemeKana", morphmeKana ?? string.Empty),
                };

                // ■ スコア計算ロジック仕様
                // 形態素辞書マッピングマスタから優先度・一致度・長さの差を考慮したスコアを計算し、スコア順にデータを取得
                // 
                // スコア = PRIORITY * 100 + 一致度ボーナス
                // 
                // 【一致度ボーナス】
                // 1. INDEXTERM = @source (元の入力文字列と完全一致)         : +100
                // 2. INDEXTERM = @morpheme (形態素と完全一致)              : +80
                // 3. INDEXTERM LIKE @morpheme + '%' (形態素の前方一致)    : +50 - (INDEXTERMの長さ - morphemeの長さ)
                // 4. READING LIKE @morphemeKana + '%' (カナの前方一致)    : +30 - (READINGの長さ - morphemeKanaの長さ)
                // 5. それ以外                                              : 0
                // 
                // 【設計意図】
                // - 完全一致を最優先(ケース1,2)
                // - 前方一致の場合、入力文字列との長さの差をペナルティとして減算(ケース3,4)
                // - これにより「急性心筋梗塞」入力時に「急性心筋梗塞後心房内血栓症」より「急性心筋梗塞」が優先される
                // - カナマッチの場合もカナの長さの差をペナルティとして適用し、より短い一致を優先
                // - 形態素の長さに応じたボーナス(1文字あたり15点)により、より長い形態素を優先
                var sql = $@"
                    SELECT TOP (@maxResults)
                        {nameof(QH_MEDISMORPHEME_MST.REFERENCECODE)},
                        {nameof(QH_MEDISMORPHEME_MST.INDEXTERM)},
                        {nameof(QH_MEDISMORPHEME_MST.READING)},
                        {nameof(QH_MEDISMORPHEME_MST.PRIORITY)},
                        CAST((
                            {nameof(QH_MEDISMORPHEME_MST.PRIORITY)} * 100 +
                            CASE 
                                WHEN {nameof(QH_MEDISMORPHEME_MST.INDEXTERM)} = @source THEN 100
                                WHEN {nameof(QH_MEDISMORPHEME_MST.INDEXTERM)} = @morpheme THEN 80
                                WHEN {nameof(QH_MEDISMORPHEME_MST.INDEXTERM)} LIKE @morpheme + '%' THEN 50 - (LEN({nameof(QH_MEDISMORPHEME_MST.INDEXTERM)}) - LEN(@morpheme))
                                WHEN {nameof(QH_MEDISMORPHEME_MST.READING)} LIKE @morphemeKana + '%' THEN 30 - (LEN({nameof(QH_MEDISMORPHEME_MST.READING)}) - LEN(@morphemeKana))
                                ELSE 0
                            END +
                            LEN(@morpheme) * 15
                        ) AS INT) AS SCORE
                    FROM  {nameof(QH_MEDISMORPHEME_MST)}
                    WHERE {nameof(QH_MEDISMORPHEME_MST.TERMTYPE)} = 1
                    AND   {nameof(QH_MEDISMORPHEME_MST.DELETEFLAG)} = 0
                    AND (
                        {nameof(QH_MEDISMORPHEME_MST.INDEXTERM)} LIKE @morpheme + '%'
                        OR {nameof(QH_MEDISMORPHEME_MST.READING)} LIKE @morphemeKana + '%'
                    )
                    ORDER BY 
                        SCORE DESC
                ";

                con.Open();

                var result = ExecuteReader<QH_MEDISMORPHEME_SCORE_VIEW>(con, null, sql, paramList);

                return result;
            }
        }

        /// <inheritdoc/>
        public List<QH_MEDISDISEASE_MST> ReadDiseaseListByScoreView(List<QH_MEDISMORPHEME_SCORE_VIEW> morphemeScoreList)
        {
            if (morphemeScoreList == null || morphemeScoreList.Count == 0)
            {
                return new List<QH_MEDISDISEASE_MST>();
            }

            using (var con = QsDbManager.CreateDbConnection<QH_MEDISDISEASE_MST>())
            {
                // morphemeScoreListをJSON配列にシリアライズ
                var jsonData = JsonConvert.SerializeObject(morphemeScoreList);

                var paramList = new List<DbParameter>()
                {
                    CreateParameter(con, "@json", jsonData)
                };

                // OPENJSONとWITH句を使用してJSON配列を展開し、QH_MEDISDISEASE_MSTと結合してSCORE順にソート
                var sql = $@"
                    SELECT d.*
                    FROM OPENJSON(@json)
                    WITH (
                        {nameof(QH_MEDISMORPHEME_SCORE_VIEW.REFERENCECODE)} nvarchar(50) '$.{nameof(QH_MEDISMORPHEME_SCORE_VIEW.REFERENCECODE)}',
                        {nameof(QH_MEDISMORPHEME_SCORE_VIEW.SCORE)} int '$.{nameof(QH_MEDISMORPHEME_SCORE_VIEW.SCORE)}'
                    ) AS json
                    INNER JOIN {nameof(QH_MEDISDISEASE_MST)} d 
                        ON d.{nameof(QH_MEDISDISEASE_MST.DISEASEEXCHANGECODE)} = json.{nameof(QH_MEDISMORPHEME_SCORE_VIEW.REFERENCECODE)}
                    ORDER BY json.{nameof(QH_MEDISMORPHEME_SCORE_VIEW.SCORE)} DESC
                ";

                con.Open();

                var result = ExecuteReader<QH_MEDISDISEASE_MST>(con, null, sql, paramList);

                return result;
            }
        }
    }
}