using MGF.QOLMS.QolmsOpenApi.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace MGF.QOLMS.QolmsOpenApi.Worker
{
    /// <summary>
    /// 診察券用のカスタムバーコード文字列を生成するヘルパー
    /// </summary>
    public static class QoPatientCardBarcodeHelper
    {
        /// <summary>
        /// カスタムバーコード文字列を生成する
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string CreateCustomBarcode(QH_PATIENTCARD_FACILITY_VIEW entity)
        {
            try
            {
                var formatItems = ParseFormat(entity.CustomCodeFormat);
                if(formatItems.Count == 0)
                {
                    throw new Exception("診察券カスタムバーコード書式が無効です。");
                }
                var barcodeSource = "";

                // 項目ごとに処理
                foreach (var (name, length) in formatItems)
                {
                    switch (name)
                    {
                        // 医療機関コードの場合
                        case "MedicalCode":
                            barcodeSource += FormatBarcodeSource(entity.MEDICALFACILITYCODE, length);
                            break;
                        // 診察券番号の場合
                        case "CardNo":
                            barcodeSource += FormatBarcodeSource(entity.CARDNO, length);
                            break;
                        default:
                            // 上記以外は何もしない
                            continue;
                    }
                }

                return barcodeSource;
            }
            catch(Exception ex)
            {
                QoAccessLog.WriteErrorLog(ex, "診察券カスタムバーコード書式のパースに失敗しました。", Guid.Empty);

                // パースに失敗はエラーとせず従来の書式で返す
                return $"{entity.MEDICALFACILITYCODE?.Trim()}{entity.CARDNO?.Trim()}";
            }
        }

        static List<(string name, int length)> ParseFormat(string format)
        {
            // 正規表現: {項目名:桁数} または {項目名} にマッチ
            var pattern = @"\{(?<name>[A-Za-z0-9_]+)(?::(?<length>\d+))?\}";
            var matches = Regex.Matches(format, pattern);

            var results = new List<(string name, int length)>();
            foreach (Match m in matches)
            {
                // 項目名と桁数に分解
                var name = m.Groups["name"].Value;
                var lengthGroup = m.Groups["length"];
                // 桁数指定がない場合は桁数フリー(-1)とする
                var length = lengthGroup.Success ? int.Parse(lengthGroup.Value) : -1;

                results.Add((name, length));
            }

            return results;
        }

        static string FormatBarcodeSource(string source, int length)
        {
            // 空白の除去
            var trimed = source.Trim();

            // 桁数フリーの場合はソースをそのまま使う
            if (length < 0)
            {
                return trimed;
            }

            // ソース末尾からlength桁分切り取る
            var extracted = trimed.Length >= length ?
                trimed.Substring(trimed.Length - length) :
                trimed;

            // 不足分は0埋めして返す
            return extracted.PadLeft(length, '0');
        }
    }
}