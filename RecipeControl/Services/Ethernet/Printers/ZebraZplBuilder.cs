using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RecipeControl.Models.DTOs;

namespace RecipeControl.Services.Ethernet.Printers
{
    public class ZebraZplBuilder
    {
        public static string BuildEtiquetaLikePhoto(
           string qrData,
           string peso,
           string codigoUnico,
           string fecha,
           string tipo,
           string sap,
           int qrMagnification = 8,
           int labelWidthDots = 800,
           int labelHeightDots = 400)
        {
            // Posiciones generales (ajustables)
            int qrX = 30, qrY = 30;
            int rightX = 420;          // columna derecha
            //int boxW = 330, boxH = 34; // tamaño de cajita
            int labelFontH = 28, labelFontW = 28;
            //int valueFontH = 26, valueFontW = 26;

            // Y de cada bloque
            int y1 = 30;
            int y2 = 105;
            int y3 = 180;
            int y4 = 255;
            int y5 = 330;

            // helper para caja + texto dentro
            string Box(int x, int y, string value)
            {
                int padX = 10;
                int padY = 7;
                return $"^FO{x + padX},{y + padY}^A0N,26,26^FD{value}^FS";
            }

            return
                "^XA" +
                "^CI28" +                                 // UTF-8 (si lo envías en UTF-8)
                $"^PW{labelWidthDots}^LL{labelHeightDots}" +

                // QR (izquierda)
                $"^FO{qrX},{qrY}^BQN,2,{qrMagnification}^FDLA,{qrData}|^FS" +

                // Derecha: Valor Pesado
                $"^FO{rightX},{y1}^A0N,{labelFontH},{labelFontW}^FDValor Pesado:^FS" +
                Box(rightX, y1 + 32, peso) +

                // Codigo Unico
                $"^FO{rightX},{y2}^A0N,{labelFontH},{labelFontW}^FDCodigo Unico:^FS" +
                Box(rightX, y2 + 32, codigoUnico) +

                // Fecha Pesado
                $"^FO{rightX},{y3}^A0N,{labelFontH},{labelFontW}^FDFecha Pesado:^FS" +
                Box(rightX, y3 + 32, fecha) +

                // Tipo
                $"^FO{rightX},{y4}^A0N,{labelFontH},{labelFontW}^FDTipo:^FS" +
                Box(rightX, y4 + 32, tipo) +

                // Codigo insumo SAP
                $"^FO{rightX},{y5}^A0N,{labelFontH},{labelFontW}^FDCodigo insumo SAP:^FS" +
                Box(rightX, y5 + 32, sap) +

                "^XZ";
        }

        public static string BuildEtiqueaVacia(
            string fecha,
            string codigoUnico,
            int qrMagnification = 8,
            int labelWidthDots = 800,
            int labelHeightDots = 400)
        {
            // Contenido del QR
            string qrData = $"MAC;{fecha};{codigoUnico}|";

            // Posición QR
            int qrX = 30;
            int qrY = 30;

            // Posición texto debajo
            int textX = 30;
            int textY = qrY + (qrMagnification * 30) + 20;
            //   ^ ajuste dinámico según magnificación del QR

            return
                "^XA" +
                "^CI28" +
                $"^PW{labelWidthDots}^LL{labelHeightDots}" +

                // QR
                $"^FO{qrX},{qrY}^BQN,2,{qrMagnification}^FDLA,{qrData}^FS" +

                // Texto debajo (fecha y código)
                $"^FO{textX},{textY}^A0N,30,30^FD{fecha};{codigoUnico}^FS" +

                "^XZ";
        }

        public static string BuildEtiquetaCompilada(
            string qrData,
            List<DataTransferQRDTO> qRDataTransferList,
            int qrMagnification = 8,
            int labelWidthDots = 800,
            int labelHeightDots = 400)
        {
            // Contenido del QR "C{1 CHAR COUNT}{X CHAR}|"
            string qrDataPrint = $"C{qrData}|";

            // Posición QR
            int qrX = 30;
            int qrY = 30;

            // Posición donde empezará el texto a la derecha del QR
            int rightX = 500;
            int startY = 40;
            int lineHeight = 40;

            var sb = new StringBuilder();

            sb.Append("^XA");
            sb.Append("^CI28");
            sb.Append($"^PW{labelWidthDots}^LL{labelHeightDots}");

            // Añadir el QR
            sb.Append($"^FO{qrX},{qrY}^BQN,2,{qrMagnification}^FDLA,{qrDataPrint}^FS");

            // Texto a la derecha
            int currentY = startY;

            // Añadir los valores de cabecera
            sb.Append($"^FO{rightX},{currentY}^A0N,30,30^FDID: {qRDataTransferList[0].MacroRegistroId.ToString()}^FS");
            currentY += lineHeight;

            sb.Append($"^FO{rightX},{currentY}^A0N,30,30^FDFE: {qRDataTransferList[0].FechaCreacion.ToString("dd/MM/yyyy HH:mm:ss")}^FS");
            currentY += lineHeight;

            sb.Append($"^FO{rightX},{currentY}^A0N,30,30^FDUS: {qRDataTransferList[0].Usuario}^FS");
            currentY += lineHeight;

            // Añadir los valores dinámicos
            foreach (var item in qRDataTransferList)
            {
                sb.Append($"^FO{rightX},{currentY}^A0N,30,30^FD{item.Codigo}: {item.PesoReal.ToString("0.00", CultureInfo.InvariantCulture)}^FS");
                    currentY += lineHeight;
            }

            sb.Append("^XZ");

            return sb.ToString();
        }

        // fecha: 26 02 2026 - IMPRIME QR opcion final 
        public static string BuildEtiquetaCompilada2(
            string qrData,
            List<DataTransfer2QRDTO> qRDataTransfer2List,
            int qrMagnification = 8,
            int labelWidthDots = 800,
            int labelHeightDots = 400)
        {
            // Contenido del QR "C{1 CHAR COUNT}{X CHAR}|"
            string qrDataPrint = $"C{qrData}|";

            // Posición QR
            int qrX = 30;
            int qrY = 30;

            // Posición donde empezará el texto a la derecha del QR
            int rightX = 500;
            int startY = 40;
            int lineHeight = 40;

            var sb = new StringBuilder();

            sb.Append("^XA");
            sb.Append("^CI28");
            sb.Append($"^PW{labelWidthDots}^LL{labelHeightDots}");

            // Añadir el QR
            sb.Append($"^FO{qrX},{qrY}^BQN,2,{qrMagnification}^FDLA,{qrDataPrint}^FS");

            // Texto a la derecha
            int currentY = startY;

            // Añadir los valores de cabecera
            sb.Append($"^FO{rightX},{currentY}^A0N,30,30^FDCod. Unico: {qRDataTransfer2List[0].MacroRegistroId.ToString()} ^FS");
            currentY += lineHeight;

            sb.Append($"^FO{rightX},{currentY}^A0N,30,30^FDPeso: {qRDataTransfer2List[0].PesoTotRealGr.ToString("0.00", CultureInfo.InvariantCulture)} g^FS"); //g para en gramos
            currentY += lineHeight;

            sb.Append($"^FO{rightX},{currentY}^A0N,30,30^FDFecha: {qRDataTransfer2List[0].FechaCreacion.ToString("dd/MM/yyyy HH:mm:ss")}^FS");
            currentY += lineHeight;

            sb.Append($"^FO{rightX},{currentY}^A0N,30,30^FDUsuario: {qRDataTransfer2List[0].Usuario}^FS");
            currentY += lineHeight;

            sb.Append($"^FO{rightX},{currentY}^A0N,30,30^FDLote: {qRDataTransfer2List[0].Lote}^FS");
            currentY += lineHeight;

            sb.Append("^XZ");

            return sb.ToString();
        }
    }
}
