﻿using DocumentFlow.Models;
using DocumentFlow.Services;
using DocumentFlow.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DocumentFlow.ViewModels
{
    class ShowHistoryPageViewModel : ViewModelBase
    {
        private readonly INavigationService navigationService;
        private readonly IMessageService messageService;
        private readonly AppDbContext db;

        private string docNumber;
        public string DocNumber { get => docNumber; set => Set(ref docNumber, value); }

        private ObservableCollection<History> historyCollection;
        public ObservableCollection<History> HistoryCollection { get => historyCollection; set => Set(ref historyCollection, value); }

        public ShowHistoryPageViewModel(INavigationService navigationService,
                                   IMessageService messageService,
                                      AppDbContext db)
        {
            this.navigationService = navigationService;
            this.messageService = messageService;
            this.db = db;
          
        }

        private RelayCommand findCommand;
        public RelayCommand FindCommand => findCommand ?? (findCommand = new RelayCommand(
                () =>
                {
                    if (!string.IsNullOrEmpty(DocNumber))
                    {
                        var trydoc = db.Documents.Where(x => x.DocNumber.Equals(DocNumber)).FirstOrDefault();
                        if (trydoc != null)
                        {
                            var doc = trydoc.Id;
                            var col = db.Histories.Where(x => x.ObjectId == doc).ToList();
                            HistoryCollection = new ObservableCollection<History>(col);
                        }
                        else
                            messageService.ShowError("Wrong number!");
                    }
                    
                }
                 ));

        string GetNextCode(string alphaCode)
        {
            Debug.Assert(alphaCode.Length == 1 && Regex.IsMatch(alphaCode, "[a-yA-y]"));

            var next = (char)(alphaCode[0] + 1);
            return next.ToString();
        }

        private RelayCommand showCommand;
        public RelayCommand ShowCommand => showCommand ?? (showCommand = new RelayCommand(
                () =>
                {
                    var app = new Application();
                    app.Visible = true;
                    app.WindowState = XlWindowState.xlMaximized;

                    Workbook wb = app.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                    Worksheet ws = wb.Worksheets[1];

                    ws.Range["A4"].Value = "Modification date:";
                    ws.Range["A5"].Value = "Modificated by:";

                    ws.Range["A6"].Value = "Document date:";
                    ws.Range["A7"].Value = "Company:";
                    ws.Range["A8"].Value = "Document type:";
                    ws.Range["A9"].Value = "Document state:";
                    ws.Range["A10"].Value = "Organization:";
                    ws.Range["A11"].Value = "Info contact:";
                    ws.Range["A12"].Value = "Sum:";
                    ws.Range["A13"].Value = "Currency:";
                    ws.Range["A14"].Value = "Comment:";
                    ws.Range["A16"].Value = "FILES";

                    var serlColl = new List<Document>();
                    
                    foreach (var item in HistoryCollection)
                    {
                        var doc = JsonConvert.DeserializeObject<Document>(item.ObjectJson);
                        serlColl.Add(doc);
                    }

                    var maxFileCount = serlColl.Max(x => x.myFiles.Count());
                    var maxProcCount = serlColl.Max(x => x.myProcesses.Count());

                    var procStart = 17 + maxFileCount;

                    ws.Range["A" + procStart].Value = "PROCESSES";

                    var letter = "A";

                    foreach (var item in serlColl)
                    {
                        letter = GetNextCode(letter);

                        ws.Range[letter + "6"].Value = item.DocDate;
                        ws.Range[letter + "7"].Value = item.Company.CompanyName;
                        ws.Range[letter + "8"].Value = item.DocumentType.DocTypeName;
                        ws.Range[letter + "9"].Value = item.DocumentState.DocStateName;
                        ws.Range[letter + "10"].Value = item.Organization.OrganizationName;
                        ws.Range[letter + "11"].Value = item.InfoContact.ToString();
                        ws.Range[letter + "12"].Value = item.DocSum;
                        ws.Range[letter + "13"].Value = item.DocCurrency.CurrancyCode;
                        ws.Range[letter + "14"].Value = item.Comment;

                        var count = 17;
                        var i = 0;

                        foreach (var fileItem in item.myFiles)
                        {
                            count = count + i;
                            ws.Range[letter + count.ToString()].Value = fileItem.FileComment + " / " + fileItem.FileName;
                        }


                        count = procStart + 1;
                        i = 0;

                        foreach (var procItem in item.myProcesses)
                        {
                            count = count + i;
                            ws.Range[letter + count.ToString()].Value = procItem.StartDate.ToShortDateString() + " / " + procItem.StartUser
                            + " / " + procItem.State + " / " + procItem.StateDate + " / " + procItem.TaskUser;
                        }


                    }


                    //DateTime currentDate = DateTime.Now;

                    //ws.Range["A1:A3"].Value = "Who is number one? :)";
                    //ws.Range["A4"].Value = "vitoshacademy.com";
                    //ws.Range["A4"].Interior.Color = XlRgbColor.rgbLightBlue;
                    //Range rng2 = ws.get_Range("A1");
                    //rng2.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Red);

                    //ws.Range["A5"].Value = currentDate;
                    //ws.Range["B6"].Value = "Tommorow's date is: =>";
                    //ws.Range["C6"].FormulaLocal = "= A5 + 1";
                    //ws.Range["A7"].FormulaLocal = "=SUM(D1:D10)";
                    //for (int i = 1; i <= 10; i++)
                    //    ws.Range["D" + i].Value = i * 2;

                    //wb.SaveAs("D:\\vitoshacademy.xlsx");

                }
                 ));

        private RelayCommand backCommand;
        public RelayCommand BackCommand => backCommand ?? (backCommand = new RelayCommand(
                () =>
                {
                    navigationService.Navigate<AdminPanelPageView>();
                }
                 ));
    }
}
