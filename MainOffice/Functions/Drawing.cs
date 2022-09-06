using MainOffice.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace MainOffice.Functions
{
    public static class Drawing
    {
        public static object DrawBill(OperDayBill bill)
        {
            /*if (billBmp != null)
            {
                billBmp.Dispose();
            }*/

            float nameA = 502.02F;
            float PictureLenthF;

            for (int i = 0; i < bill.BillLines.Count; i++)
            {
                //Наименование позиции
                int zeroCount;
                string[] item = AllocateString(addZeroToCode(bill.BillLines[i].CashRegCode.ToString(), out zeroCount) + " " + bill.BillLines[i].ProductOrServiceName);
                item[0] = item[0].Substring(zeroCount);

                nameA = nameA + (44.57F * (item.Count() + 2)) + 10F;

            }
            PictureLenthF = nameA + 262.59F;

            string PictureLenthFloat = PictureLenthF.ToString();
            if (PictureLenthFloat.IndexOf(",") != -1)
            {
                PictureLenthFloat = PictureLenthFloat.Substring(0, PictureLenthFloat.IndexOf(","));
            }

            int PictureLenth = int.Parse(PictureLenthFloat) + 1;
            Bitmap billBmp = new Bitmap(782, PictureLenth);

            billBmp.SetResolution(300F, 300F);
            Graphics billGraphics = Graphics.FromImage(billBmp);
            Brush textBrush;
            Font arial6, arial8, arial12, arial14, cSCBook12, cSCBook11, cSCBook10, cGoth14;
            //if (addDate)
            //{
            textBrush = Brushes.Black;
            arial6 = new Font("Arial", 6, FontStyle.Regular);
            arial8 = new Font("Arial", 8, FontStyle.Regular);
            arial12 = new Font("Arial", 12, FontStyle.Regular);
            cSCBook12 = new Font("Century Schoolbook", 12, FontStyle.Regular);
            cSCBook11 = new Font("Century Schoolbook", 11, FontStyle.Regular);
            cSCBook10 = new Font("Century Schoolbook", 10, FontStyle.Regular);
            cGoth14 = new Font("Century Gothic", 14, FontStyle.Regular);

            /*}
            else
            {
                arial6 = new Font("Arial", 6, FontStyle.Bold);
                arial8 = new Font("Arial", 8, FontStyle.Bold);
                arial12 = new Font("Arial", 12, FontStyle.Bold);
                cSCBook12 = new Font("Century Schoolbook", 12, FontStyle.Bold);
                cSCBook11 = new Font("Century Schoolbook", 11, FontStyle.Bold);
                cSCBook10 = new Font("Century Schoolbook", 10, FontStyle.Bold);
                cGoth14 = new Font("Century Gothic", 14, FontStyle.Bold);
                textBrush = Brushes.Black;
            }*/
            Font arial12r = new Font("Arial", 12, FontStyle.Regular);
            arial14 = new Font("Arial", 14, FontStyle.Bold);
            Font arial12b = new Font("Arial", 12, FontStyle.Bold);
            Font arial8b = new Font("Arial", 8, FontStyle.Bold);
            RectangleF Place = new RectangleF(0F, 34.38F, 782F, 61.71F);
            StringFormat strFormat = new StringFormat();
            strFormat.Alignment = StringAlignment.Center;
            strFormat.LineAlignment = StringAlignment.Center;
            Pen linePen = new Pen(textBrush, 4.18F);
            //РАХУНОК
            billGraphics.DrawString("РАХУНОК", arial12, textBrush, Place, strFormat);
            //МАЙСТЕР
            //Place.Y = 182.14F;
            //billGraphics.DrawString("МАЙСТЕР", arial12, textBrush, Place, strFormat);
            //ЗНИЖКА 10%
            Place.Y = 300.41F;
            billGraphics.DrawString("НА ВСІ ПОСЛУГИ САЛОНУ", arial12, textBrush, Place, strFormat);
            //ДІЙСНА ДО
            Place.Y = Place.Y + 63.87F;
            billGraphics.DrawString("ЗНИЖКА 10% ДІЙСНА ДО", arial12, textBrush, Place, strFormat);
            //ДАТА ЗНИЖКА 10% ДІЙСНА ДО
            Place.Y = Place.Y + 63.87F;
            billGraphics.DrawString(AddZero(DateTime.Now.AddDays(30).Day.ToString()) + " / " + AddZero(DateTime.Now.AddDays(30).Month.ToString()) + " / " + DateTime.Now.AddDays(30).Year.ToString(), cSCBook12, textBrush, Place, strFormat);


            /*
            //САЛОН
            Place.Y = 300.41F;
            XmlDocument salonXml = (XmlDocument)Session["salonXml"];
            XmlNode node = salonXml.SelectSingleNode("/salon/item[@ID='" + settingsXml.SelectSingleNode("/settings/salon").Attributes["ID"].Value + "']");

            if (node == null)
            {
                MessageDisplay.DisplayMessage("Помилка", "Даний салон не існує\rПрограма закінчує працювати\r");
                //MessageBox.Show("Даний салон не існує\rПрограма закінчує працювати\r", "Помилка");
                Environment.Exit(0);
            }

            billGraphics.DrawString(node.Attributes["TYPE"].Value, arial12, textBrush, Place, strFormat);
            //"ЛОНДА"
            Place.Y = Place.Y + 56.83F;
            billGraphics.DrawString("ЛОНDА", cGoth14, textBrush, Place, strFormat);
            //Метро
            Place.Y = Place.Y + 56.83F;
            billGraphics.DrawString(node.Attributes["SUB"].Value, cSCBook11, textBrush, Place, strFormat);
            //адрес
            Place.Y = Place.Y + 56.83F;
            billGraphics.DrawString(node.Attributes["ADR"].Value, cSCBook11, textBrush, Place, strFormat);
            //телефон 1
            Place.Y = Place.Y + 56.83F;
            billGraphics.DrawString(node.Attributes["TEL1"].Value, cSCBook11, textBrush, Place, strFormat);
            //телефон 2
            Place.Y = Place.Y + 56.83F;
            billGraphics.DrawString(node.Attributes["TEL2"].Value, cSCBook11, textBrush, Place, strFormat);
            */

            //номер счета
            Place.Y = 94.80F;

            string printNum = bill.BillNumber.ToString() + DateTime.Now.Year + AddZero(DateTime.Now.Month) + AddZero(DateTime.Now.Day) + AddZero(DateTime.Now.Hour) + AddZero(DateTime.Now.Minute);
            billGraphics.DrawString(printNum, arial12, textBrush, Place, strFormat);


            //горизонтальные линии
            billGraphics.DrawLine(linePen, 0F, 154.41F, 782F, 154.41F);
            billGraphics.DrawLine(linePen, 0F, 290.41F, 782F, 290.41F);
            billGraphics.DrawLine(linePen, 0F, 492.02F, 782F, 492.02F);



            decimal wholeSumm = 0;
            decimal wholeSummWithoutDiscount = 0;
            float wholeSummY = 900.41F;

            //Фамилия и имя
            Place.Y = 168.86F;
            billGraphics.DrawString(bill.OperationDayEmployee.Employee.FamilyName + " " + bill.OperationDayEmployee.Employee.Name, cSCBook11, textBrush, Place, strFormat);

            // Звание и номер
            Place.Y = 225.30F;
            billGraphics.DrawString(bill.OperationDayEmployee.Employee.BarberLevel != null ? bill.OperationDayEmployee.Employee.BarberLevel.Name : bill.OperationDayEmployee.Employee.Profession.Name + " " + bill.OperationDayEmployee.Employee.StaffNumber, cSCBook10, textBrush, Place, strFormat);
            //Позиции прайс листа


            float nameX = 5.80F;
            float nameY = 502.02F;
            float nameW = 587.33F;
            float nameH;
            /*float priceW = 160.25F;
            float priceH = 40.57F;
            float priceX = 381.69F;
            float priceY;
            float discountX = 526.94F;
            float discountY;
            float discountW = 80F;
            float discountH = 40.57F;
            float summX = 590.94F;
            float summY;
            float summW = 191.06F;
            float summH = 72F;*/


            for (int i = 0; i < bill.BillLines.Count; i++)
            {
                if (bill.BillLines[i].Cancel)
                    continue;
                //Наименование позиции
                int zeroCount;
                string[] item = AllocateString(addZeroToCode(bill.BillLines[i].CashRegCode.ToString(), out zeroCount) + " " + bill.BillLines[i].ProductOrServiceName);
                //string[] item = AllocateString(currentWrkshXml.ChildNodes[i].Attributes["ID"].Value + " " + currentWrkshXml.ChildNodes[i].Attributes["NAME"].Value);
                item[0] = item[0].Substring(zeroCount);
                /*if (item.Count() < 3)
                {
                    nameH = 40.57F * 3.5F / 1.5F;
                }
                else
                {
                    nameH = 40.57F * item.Count();
                }*/

                nameH = 44.57F * (item.Count() + 2);
                /*if ((nameY + nameH) > 1829.41F)
                {
                    //удалить последнюю строку
                    DropLastItem();
                    Session["resultNode"] = null;
                    MessageBox messageBox = new MessageBox();
                    messageBox.MessageText = "Неможливо додати останню позицію\nНевистачає місця для друку\nНадрукуйте ще один рахунок";
                    messageBox.MessageTitle = "Увага";
                    messageBox.MessageButtons = MessageBox.MessageBoxButtons.Ok;
                    messageBox.MessageIcons = MessageBox.MessageBoxIcons.Warnning;
                    Literal1.Text = messageBox.Show(this);

                    //MessageDisplay.DisplayMessage("Неможливо додати останню позицію", "Невистачає місця для друку<br>Надрукуйте ще один рахунок", "~/staffonly/Default.aspx", 5);
                    //MessageBox.Show("Неможливо додати останню позицію\rНевистачає місця для друку\rНадрукуйте ще один рахунок\r", "Помилка");
                    break;
                }*/
                wholeSummY = nameY + nameH;
                Place.X = nameX;
                Place.Y = nameY;
                Place.Width = nameW;
                Place.Height = 44.57F;
                strFormat.LineAlignment = StringAlignment.Center;
                strFormat.Alignment = StringAlignment.Near;

                /*if (addDate)
                {*/

                //billGraphics.FillRectangle(Brushes.DarkOliveGreen, Place);
                for (int f = 0; f < item.Count(); f++)
                {
                    string[] temp = new string[1];
                    if (f == 0)
                    {

                        if (item[f].IndexOf(" ") == -1)
                        {
                            temp[0] = item[f];
                            Place.Width = 150.49F;
                            DrawSpacedText(billGraphics, temp, arial12r, textBrush, Place, strFormat, 5F, 44.57F);
                            Place.X = nameX;
                            Place.Width = nameW;
                        }
                        else
                        {
                            temp[0] = item[f].Substring(0, item[f].IndexOf(" "));
                            Place.Width = 150.49F;
                            DrawSpacedText(billGraphics, temp, arial12r, textBrush, Place, strFormat, 5F, 44.57F);
                            Place.X = 150.49F;
                            Place.Width = 436.84F;
                            temp[0] = item[f].Substring(item[f].IndexOf(" ") + 1);
                            DrawSpacedText(billGraphics, temp, arial12r, textBrush, Place, strFormat, 5F, 44.57F);
                            Place.X = nameX;
                            Place.Width = nameW;
                        }
                    }
                    else
                    {
                        temp[0] = item[f];
                        //billGraphics.FillRectangle(Brushes.AntiqueWhite, Place);
                        DrawSpacedText(billGraphics, temp, arial12r, textBrush, Place, strFormat, 5F, 44.57F);
                    }
                    Place.Y = Place.Y + 44.57F;
                }
                //DrawSpacedText(billGraphics, item, arial6, textBrush, Place, strFormat, 5F, 40.57F);

                /*}
                else
                {
                    for (int f = 0; f < item.Count(); f++)
                    {
                        if (f == 0)
                        {
                            Place.Width = 90.49F;
                            billGraphics.DrawString(item[f].Substring(0, item[f].IndexOf(" ")), arial12r, textBrush, Place, strFormat);
                            Place.X = 90.49F;
                            Place.Width = 305.4F;
                            billGraphics.DrawString(item[f].Substring(item[f].IndexOf(" ") + 1), arial12r, textBrush, Place, strFormat);
                            Place.X = nameX;
                            Place.Width = nameW;
                        }
                        else
                        {
                            billGraphics.DrawString(item[f], arial8, textBrush, Place, strFormat);
                        }
                        Place.Y = Place.Y + 40.57F;
                    }
                    //billGraphics.DrawString(itemToDisplay, arial8, textBrush, Place, strFormat);
                }*/
                //strFormat.LineAlignment = StringAlignment.Center;
                //strFormat.Alignment = StringAlignment.Center;

                // Цена и количество + сумма без скидки
                /*priceY = nameY + (nameH / 2) - (3.5F / 3F * priceH);
                Place.X = priceX;
                Place.Y = priceY;
                Place.Width = priceW;
                Place.Height = priceH;*/

                Place.Width = 200F;
                //billGraphics.FillRectangle(Brushes.AntiqueWhite, Place);
                billGraphics.DrawString(bill.BillLines[i].Quantity + ".000", arial12r, textBrush, Place, strFormat);
                Place.X = Place.X + 200F;
                Place.Width = 40F;
                //billGraphics.FillRectangle(Brushes.Blue, Place);
                billGraphics.DrawString("X", arial12r, textBrush, Place, strFormat);
                Place.X = Place.X + 50F;
                Place.Width = 250.33F;
                //billGraphics.FillRectangle(Brushes.Brown, Place);
                billGraphics.DrawString(addZeroToSumm(bill.BillLines[i].SellPrice.ToString()) + "=", arial12r, textBrush, Place, strFormat);
                //Place.Y = Place.Y + priceH / 1.5F;
                //billGraphics.FillRectangle(Brushes.Blue, Place);
                //billGraphics.DrawString("=", arial8, textBrush, Place, strFormat);
                //Place.Y = Place.Y + priceH / 1.5F;
                Place.X = Place.X + 260.33F;
                Place.Width = 255.87F;
                //billGraphics.FillRectangle(Brushes.AntiqueWhite, Place);
                decimal summWithoutDiscount = bill.BillLines[i].MaxPrice * bill.BillLines[i].Quantity;
                wholeSummWithoutDiscount = wholeSummWithoutDiscount + summWithoutDiscount;
                strFormat.Alignment = StringAlignment.Far;
                billGraphics.DrawString(addZeroToSumm((summWithoutDiscount).ToString()), arial12r, textBrush, Place, strFormat);


                if (bill.BillLines[i].MaxPrice != bill.BillLines[i].SellPrice)
                {
                    // Знижка
                    /*discountY = nameY + (nameH / 2) - discountH / 2;
                    Place.X = discountX;
                    Place.Y = discountY;
                    Place.Width = discountW;
                    Place.Height = discountH;*/
                    Place.Y = Place.Y + 44.57F;
                    Place.X = nameX;
                    Place.Width = 500.33F;
                    //billGraphics.FillRectangle(Brushes.Coral, Place);
                    strFormat.Alignment = StringAlignment.Near;
                    billGraphics.DrawString("ЗНИЖКА: " + bill.BillLines[i].Promotion, arial12r, textBrush, Place, strFormat);
                    Place.X = Place.X + 510.33F;
                    Place.Width = 255.87F;
                    strFormat.Alignment = StringAlignment.Far;
                    billGraphics.DrawString("-" + addZeroToSumm((bill.BillLines[i].MaxPrice * bill.BillLines[i].Quantity - bill.BillLines[i].SellPrice * bill.BillLines[i].Quantity).ToString()), arial12r, textBrush, Place, strFormat);
                }

                //Сумма 
                /*summY = discountY - 15.72F;
                Place.X = summX;
                Place.Y = summY;
                Place.Width = summW;
                Place.Height = summH;
                //billGraphics.FillRectangle(Brushes.AntiqueWhite, Place);
                billGraphics.DrawString((decimal.Parse(currentWrkshXml.ChildNodes[i].Attributes["SELLPRICE"].Value) * decimal.Parse(currentWrkshXml.ChildNodes[i].Attributes["QUANTITY"].Value)).ToString(), arial12r, Brushes.Black, Place, strFormat);
                */
                wholeSumm = wholeSumm + (bill.BillLines[i].SellPrice * bill.BillLines[i].Quantity);

                nameY = nameY + nameH + 10F;

            }

            /*
        //горизонтальная пунктирная линия с ножницами

        billGraphics.DrawLine(linePen, 136.22F, 806.41F, 182.67F, 806.41F);
        billGraphics.DrawLine(linePen, 187.67F, 806.41F, 234.13F, 806.41F);
        billGraphics.DrawLine(linePen, 239.13F, 806.41F, 285.58F, 806.41F);
        billGraphics.DrawLine(linePen, 290.58F, 806.41F, 337.04F, 806.41F);
        billGraphics.DrawLine(linePen, 342.04F, 806.41F, 388.5F, 806.41F);
        billGraphics.DrawLine(linePen, 393.5F, 806.41F, 439.95F, 806.41F);
        billGraphics.DrawLine(linePen, 444.95F, 806.41F, 491.41F, 806.41F);
        billGraphics.DrawLine(linePen, 496.41F, 806.41F, 542.86F, 806.41F);
        billGraphics.DrawLine(linePen, 547.86F, 806.41F, 594.32F, 806.41F);
        billGraphics.DrawLine(linePen, 599.32F, 806.41F, 645.78F, 806.41F);

        billGraphics.DrawLine(linePen, 650.78F, 806.41F, 697.24F, 806.41F);
        billGraphics.DrawLine(linePen, 702.24F, 806.41F, 748.7F, 806.41F);
        billGraphics.DrawLine(linePen, 753.7F, 806.41F, 782F, 806.41F);

        //ножницы левые
        Place.X = 0F;
        Place.Width = 200F;
        Place.Y = 746.41F;
        Place.Height = 100F;
        RectangleF scissorsRect = new RectangleF(0, 0, 136F, 68F);
        if (addDate)
        {
            billGraphics.DrawImage(LondaBillWeb.Properties.Resources.scissorsgray, Place, scissorsRect, GraphicsUnit.Pixel);
        }
        else
        {
            billGraphics.DrawImage(LondaBillWeb.Properties.Resources.scissors, Place, scissorsRect, GraphicsUnit.Pixel);
        }*/
            //горизонтальная линия
            billGraphics.DrawLine(linePen, 0F, wholeSummY, 782F, wholeSummY);
            //ВАРТІСТЬ
            Place.Y = wholeSummY + 10.56F;// 1956.97F;
            Place.X = 0F;
            Place.Width = 391F;
            Place.Height = 47.18F;
            strFormat.Alignment = StringAlignment.Near;
            billGraphics.DrawString("ВАРТІСТЬ", arial12r, textBrush, Place, strFormat);
            //Сумма ВАРТІСТЬ
            Place.X = 302F;
            Place.Width = 481F;
            strFormat.Alignment = StringAlignment.Far;
            if (wholeSummWithoutDiscount == 0)
            {
                billGraphics.DrawString("0 грн. 00 коп.", arial12r, textBrush, Place, strFormat);
            }
            else
            {
                if (wholeSummWithoutDiscount.ToString().IndexOf(",") == -1)
                {
                    billGraphics.DrawString(wholeSummWithoutDiscount.ToString() + " грн. 00 коп.", arial12r, textBrush, Place, strFormat);
                }
                else
                {
                    if (wholeSummWithoutDiscount.ToString().Substring(wholeSummWithoutDiscount.ToString().IndexOf(",") + 1).Length == 1)
                    {
                        billGraphics.DrawString(wholeSummWithoutDiscount.ToString().Substring(0, wholeSummWithoutDiscount.ToString().IndexOf(",")) + " грн. " + wholeSummWithoutDiscount.ToString().Substring(wholeSummWithoutDiscount.ToString().IndexOf(",") + 1) + "0 коп.", arial12r, textBrush, Place, strFormat);
                    }
                    else
                    {
                        billGraphics.DrawString(wholeSummWithoutDiscount.ToString().Substring(0, wholeSummWithoutDiscount.ToString().IndexOf(",")) + " грн. " + wholeSummWithoutDiscount.ToString().Substring(wholeSummWithoutDiscount.ToString().IndexOf(",") + 1) + " коп.", arial12r, textBrush, Place, strFormat);
                    }
                }
            }
            //billGraphics.FillRectangle(Brushes.Blue, Place);
            wholeSummY = Place.Y + Place.Height + 5F;

            //горизонтальная линия
            billGraphics.DrawLine(linePen, 0F, wholeSummY, 782F, wholeSummY);
            //Знижка
            Place.Y = wholeSummY + 10.56F;// 1956.97F;
            Place.X = 0F;
            Place.Width = 391F;
            Place.Height = 47.18F;
            strFormat.Alignment = StringAlignment.Near;
            billGraphics.DrawString("ЗНИЖКА", arial12r, textBrush, Place, strFormat);
            //Сумма Знижки
            decimal summDiscount = wholeSummWithoutDiscount - wholeSumm;
            Place.X = 302F;
            Place.Width = 481F;
            strFormat.Alignment = StringAlignment.Far;
            if (summDiscount == 0)
            {
                billGraphics.DrawString("0 грн. 00 коп.", arial12r, textBrush, Place, strFormat);
            }
            else
            {
                if (summDiscount.ToString().IndexOf(",") == -1)
                {
                    billGraphics.DrawString("-" + summDiscount.ToString() + " грн. 00 коп.", arial12r, textBrush, Place, strFormat);
                }
                else
                {
                    if (summDiscount.ToString().Substring(summDiscount.ToString().IndexOf(",") + 1).Length == 1)
                    {
                        billGraphics.DrawString("-" + summDiscount.ToString().Substring(0, summDiscount.ToString().IndexOf(",")) + " грн. " + summDiscount.ToString().Substring(summDiscount.ToString().IndexOf(",") + 1) + "0 коп.", arial12r, textBrush, Place, strFormat);
                    }
                    else
                    {
                        billGraphics.DrawString("-" + summDiscount.ToString().Substring(0, summDiscount.ToString().IndexOf(",")) + " грн. " + summDiscount.ToString().Substring(summDiscount.ToString().IndexOf(",") + 1) + " коп.", arial12r, textBrush, Place, strFormat);
                    }
                }
            }
            //billGraphics.FillRectangle(Brushes.Blue, Place);
            wholeSummY = Place.Y + Place.Height + 5F;

            //горизонтальная линия
            billGraphics.DrawLine(linePen, 0F, wholeSummY, 782F, wholeSummY);
            //"ВСЬОГО ДО СПЛАТИ"
            Place.Y = wholeSummY + 18.56F;// 1956.97F;
            Place.X = 0F;
            Place.Width = 391F;
            Place.Height = 57.18F;
            strFormat.Alignment = StringAlignment.Near;
            billGraphics.DrawString("ДО СПЛАТИ", arial14, textBrush, Place, strFormat);
            Place.Y = Place.Y + 57.18F;
            billGraphics.DrawString("ВСЬОГО", arial14, textBrush, Place, strFormat);

            //сумма всего
            Place.X = 252F;
            Place.Width = 531F;
            Place.Y = wholeSummY + 18.56F + 57.18F;
            Place.Height = 57.18F;
            strFormat.Alignment = StringAlignment.Far;
            //billGraphics.FillRectangle(Brushes.Blue, Place);
            if (wholeSumm == 0)
            {
                billGraphics.DrawString("0 грн. 00 коп.", arial12r, Brushes.Black, Place, strFormat);

            }
            else
            {

                if (wholeSumm.ToString().IndexOf(",") == -1)
                {
                    billGraphics.DrawString(wholeSumm.ToString() + " грн. 00 коп.", arial12r, Brushes.Black, Place, strFormat);
                }
                else
                {
                    if (wholeSumm.ToString().Substring(wholeSumm.ToString().IndexOf(",") + 1).Length == 1)
                    {
                        billGraphics.DrawString(wholeSumm.ToString().Substring(0, wholeSumm.ToString().IndexOf(",")) + " грн. " + wholeSumm.ToString().Substring(wholeSumm.ToString().IndexOf(",") + 1) + "0 коп.", arial12r, Brushes.Black, Place, strFormat);
                    }
                    else
                    {
                        billGraphics.DrawString(wholeSumm.ToString().Substring(0, wholeSumm.ToString().IndexOf(",")) + " грн. " + wholeSumm.ToString().Substring(wholeSumm.ToString().IndexOf(",") + 1) + " коп.", arial12r, Brushes.Black, Place, strFormat);
                    }
                }

            }


            strFormat.Alignment = StringAlignment.Center;
            /*//Ціна
            Place.X = 396.69F;
            Place.Y = 837.12F;
            Place.Width = 102.75F;
            Place.Height = 41.15F;
            billGraphics.DrawString("ЦІНА", arial8, textBrush, Place, strFormat);
            //Знижка
            Place.X = 499.44F;
            Place.Y = 844.27F;
            Place.Width = 120.77F;
            Place.Height = 30.86F;
            billGraphics.DrawString("ЗНИЖКА", arial6, textBrush, Place, strFormat);
            //Сума
            Place.X = 620.21F;
            Place.Y = 837.12F;
            Place.Width = 131.79F;
            Place.Height = 41.15F;
            billGraphics.DrawString("СУМА", arial8, textBrush, Place, strFormat);
            */
            billGraphics.Dispose();

            //Вывод на экран рисунка

            Bitmap billToMonitor = new Bitmap(281, 750);
            billToMonitor.SetResolution(300F, 300F);
            RectangleF destRectangle = new RectangleF(0F, 0F, 281F, 750F);
            RectangleF sourceRectangle = new RectangleF(0F, 0F, 782F, PictureLenth);
            Graphics billToMonitorGraphics = Graphics.FromImage(billToMonitor);
            billToMonitorGraphics.DrawImage(billBmp, destRectangle, sourceRectangle, GraphicsUnit.Pixel);
            billToMonitorGraphics.Dispose();
            PictureLenthFloat = (PictureLenth / 1.379).ToString();
            if (PictureLenthFloat.IndexOf(",") != -1)
            {
                PictureLenthFloat = PictureLenthFloat.Substring(0, PictureLenthFloat.IndexOf(","));
            }
            PictureLenth = int.Parse(PictureLenthFloat);
            Bitmap billToPrint = new Bitmap(567, PictureLenth);
            billToPrint.SetResolution(300F, 300F);
            destRectangle = new RectangleF(0F, 0F, 567F, PictureLenth);
            billToMonitorGraphics = Graphics.FromImage(billToPrint);
            billToMonitorGraphics.DrawImage(billBmp, destRectangle, sourceRectangle, GraphicsUnit.Pixel);
            billToMonitorGraphics.Dispose();

            //Session["billBmp"] = billToPrint;
            ImageConverter converter = new ImageConverter();
            //Session["billToMonitor"] = converter.ConvertTo(billToMonitor, typeof(byte[]));
            return converter.ConvertTo(billToPrint, typeof(byte[]));
            //ImageBill.ImageUrl = "~/staffonly/ImageHandler1.ashx";
            //pictureBoxBill.Image = billToMonitor;
        }
        private static string[] AllocateString(string input)
        {
            string[] result = new string[1];
            string[] newLine = new string[1];
            string Code = "";

            //Убираем лишние пробелы
            int startIntSpace = 0;
            Char space = new Char();
            space = (" ").ToCharArray()[0];
            bool textStarted = false;
            string text = null;
            for (int i = 0; i < input.Length; i++)
            {

                if (input[i] == space & textStarted)
                {
                    if (startIntSpace == 0)
                    {
                        text = input.Substring(startIntSpace, i);
                    }
                    else
                    {
                        text = text + " " + input.Substring(startIntSpace, i - startIntSpace);
                    }
                    startIntSpace = i + 1;
                    textStarted = false;
                }
                else
                {
                    if (input[i] != space)
                    {
                        textStarted = true;
                        if (i == input.Length - 1)
                        {
                            text = text + " " + input.Substring(startIntSpace);
                        }

                    }
                    else
                    {
                        startIntSpace = startIntSpace + 1;
                    }
                }
            }
            input = text;

            //Разбиваем на строки
            if (input.Length > 19)
            {
                bool start = true;
                while (input.Length > 19)
                {
                    int startIndex = 0;
                    if (input.IndexOf(" ", startIndex) > 19 | input.IndexOf(" ", startIndex) == -1)
                    {
                        input = Code + " В назві присутнє слово більше 19 символів в довжину";
                        result = new string[1];
                        start = true;
                    }

                    while (input.IndexOf(" ", startIndex) < 20 & input.IndexOf(" ", startIndex) != -1)
                    {
                        if (start)
                        {

                            if (startIndex == 0)
                            {
                                result[0] = input.Substring(startIndex, input.IndexOf(" ", startIndex));
                                Code = result[0];
                            }
                            else
                            {
                                result[0] = result[0] + " " + input.Substring(startIndex, input.IndexOf(" ", startIndex) - startIndex);
                            }
                        }
                        else
                        {
                            if (startIndex == 0)
                            {

                                newLine[0] = input.Substring(startIndex, input.IndexOf(" ", startIndex));
                            }
                            else
                            {
                                newLine[0] = newLine[0] + " " + input.Substring(startIndex, input.IndexOf(" ", startIndex) - startIndex);
                            }
                        }

                        startIndex = input.IndexOf(" ", startIndex) + 1;
                    }
                    if (!start)
                    {
                        result = result.Concat(newLine).ToArray();
                    }
                    start = false;

                    input = input.Substring(startIndex);
                }
                newLine[0] = input;
                result = result.Concat(newLine).ToArray();
            }
            else
            {
                result[0] = input;
            }
            return result;
        }

        //Добавляем ноли к коду товара
        private static string addZeroToCode(string input, out int count)
        {
            string result;
            switch (input.Length)
            {
                case 1:
                    result = "000" + input;
                    count = 3;
                    break;
                case 2:
                    result = "00" + input;
                    count = 2;
                    break;
                case 3:
                    result = "0" + input;
                    count = 1;
                    break;
                default:
                    result = input;
                    count = 0;
                    break;
            }
            return result;
        }
        /// <summary>
        /// Добавляет ноль к строке, спереди, если длинна строки = 1
        /// </summary>
        /// <param name="input">Строка для проверки</param>
        /// <returns>string</returns>
        private static string AddZero(string input)
        {
            if (input.Length == 1)
            {
                input = "0" + input;
            }
            return input;
        }

        /// <summary>
        /// Добавляет ноль к строке, спереди, если длинна строки = 1
        /// </summary>
        /// <param name="input">Строка для проверки</param>
        /// <returns>string</returns>
        private static string AddZero(int input)
        {
            if (input.ToString().Length == 1)
            {
                return "0" + input.ToString();
            }
            else
            {
                return input.ToString();
            }

        }
        //Рисование текста с интервалами между буквами
        private static void DrawSpacedText(Graphics g, string[] text, Font font, Brush brush, RectangleF rectangle, StringFormat strFormat, float desiredWidth, float strHight)
        {
            float lineY = rectangle.Y;
            if (strFormat.LineAlignment == StringAlignment.Center)
            {
                lineY = (rectangle.Y + rectangle.Height / 2) - (strHight * text.Count() / 2);
            }

            for (int z = 0; z < text.Count(); z++)
            {
                CharacterRange[] ranges = null;
                for (int i = 0; i < text[z].Length; i++)
                {

                    if (i == 0)
                    {
                        ranges = new CharacterRange[] { new CharacterRange(i, 1) };
                    }
                    else
                    {
                        ranges = ranges.Concat(new CharacterRange[] { new CharacterRange(i, 1) }).ToArray();
                    }
                }



                strFormat.SetMeasurableCharacterRanges(ranges);
                //Calculate spacing
                float widthNeeded = 0;
                int intChar, intLines;
                SizeF sizeStr = g.MeasureString(text[z], font, rectangle.Size, strFormat, out intChar, out intLines);
                Region[] regions = g.MeasureCharacterRanges(text[z], font, rectangle, strFormat);

                float[] widths = new float[regions.Length];
                for (int i = 0; i < widths.Length; i++)
                {
                    widths[i] = regions[i].GetBounds(g).Width;
                    widthNeeded += widths[i];
                }
                float spacing = desiredWidth;

                //draw text
                float indent = 0;
                int index = 0;
                RectangleF place = new RectangleF();
                foreach (char c in text[z])
                {
                    place.X = rectangle.X + indent;
                    place.Y = lineY;
                    place.Width = widths[index] + spacing;
                    place.Height = strHight;
                    g.DrawString(c.ToString(), font, brush, place, strFormat);
                    indent += widths[index] + spacing;
                    index++;
                }


                lineY = lineY + strHight;
            }
        }
        private static string addZeroToSumm(string input)
        {
            string result;
            int y = input.IndexOf(",");
            if (y == -1)
            {
                result = input + ".00";
            }
            else
            {
                if (input.Substring(y).Length == 2)
                {
                    result = input.Replace(",", ".") + "0";
                }
                else
                {
                    result = input.Replace(",", ".");
                }
            }
            return result;
        }
    }
}