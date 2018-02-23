using TFOF.Areas.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace TFOF.Areas.Core.Services
{
	public class AppService
	{
		public bool CreateApp( HttpContextBase httpContext, AppModel model)
		{
			if (model == null)
			{
				return false;
			}
			string TemplatePath = httpContext.Server.MapPath(@"~\App_Data\_Template\");
			string AreaPath = httpContext.Server.MapPath(@"~\App_Data\" + model.Area + @"\");

			//List<string> dirs = new List<string>(Directory.EnumerateDirectories(TemplatePath));
			List<string> paths = new List<string>();
			Queue<string> fileQueue = new Queue<string>();
			Queue<string> dirQueue = new Queue<string>();
			dirQueue.Enqueue(TemplatePath);

			while (dirQueue.Count > 0)
			{
				//Craft the new location of the directory
				string templateDir = dirQueue.Dequeue();
				string areaDir = templateDir.Replace(TemplatePath, AreaPath).Replace("_Template", model.Name);

				List<string> files = new List<string>(Directory.EnumerateFiles(templateDir));
				foreach (string file in files)
				{
					fileQueue.Enqueue(file);
					paths.Add(file);
				}
				paths.Add(areaDir);
				if (!Directory.Exists(areaDir))
				{
					Directory.CreateDirectory(areaDir);
				}
				List<string> subdirs = new List<string>(Directory.EnumerateDirectories(templateDir));
				foreach (string subdir in subdirs)
				{
					dirQueue.Enqueue(subdir);
				}
			}

			///Loops through the fields that will rendered in the model and forms
			string modelFields = "";
			string searchFields = "";
			string sortFields = "";
			string formFields = "";
			string indexPageHeaders = "";
			string indexPageColumns = "";
			string sqlColumns = "";
			List<string> modelUsingList = new List<string>();
			List<string> expandList = new List<string>();
			List<string> sqlKeyList = new List<string>();

			Dictionary<string, string> ints = new Dictionary<string, string>()
			{
				{ AppFieldModel.Int16Type, "smallint" },
				{ AppFieldModel.intType, "int" },
				{ AppFieldModel.Int64Type, "bigint" },
				{ AppFieldModel.Int16NullableType, "smallint" },
				{ AppFieldModel.intNullableType, "int" },
				{ AppFieldModel.Int64NullableType, "bigint" },
			};

			foreach (var item in model.AppFields.OrderBy(o => o.Position).Select((field, i) => new { i, field }))
			// foreach (AppFieldModel field in model.AppFields.OrderBy(o => o.Position))
			{
				//ModelFields
				AppFieldModel field = item.field;
				if (field.IsPrimaryKey)
				{
					modelFields += "\t\t[Key]\r\n";
				}
				if (!string.IsNullOrWhiteSpace(field.ColumnName) && !field.Name.Equals(field.ColumnName))
				{
					modelFields += "\t\t[Column(\"" + field.ColumnName + "\")]\r\n";
				}
				if (field.DataType.Equals(AppFieldModel.stringType) && field.StringLength != null)
				{
					modelFields += "\t\t[StringLength(" + field.StringLength.ToString() + ")]\r\n";
				}
				if (!string.IsNullOrWhiteSpace(field.ForeignKeyTo))
				{
					modelFields += "\t\t[ForeignKey(\"" + field.ForeignKeyTo.Split('.').Last().Replace("Model", "") + "\")]\r\n";
				}

				if (!string.IsNullOrWhiteSpace(field.DisplayName))
				{
					modelFields += "\t\t[Display(Name = \"" + field.DisplayName + "\")]\r\n";
				}

				modelFields += "\t\tpublic " + field.DataType + " " + field.Name + " { get; set; }\r\n";

				if (!string.IsNullOrWhiteSpace(field.ForeignKeyTo))
				{
					string modelUsing = "using " + field.ForeignKeyTo.Replace("." + field.ForeignKeyTo.Split('.').Last(), ";");
					if (!modelUsingList.Contains(modelUsing))
					{
						modelUsingList.Add(modelUsing);
					}
					modelFields += "\t\tpublic virtual " + field.ForeignKeyTo.Split('.').Last() + " " + field.ForeignKeyTo.Split('.').Last().Replace("Model", "") + " { get; set; }\r\n";
					expandList.Add(field.ForeignKeyTo.Split('.').Last().Replace("Model", ""));
				}
				modelFields += "\r\n";


				//Search & Sort Fields
				if (field.IsSearchable)
				{
					if (field.DataType.Equals(AppFieldModel.DateTimeNullableType) || field.DataType.Equals(AppFieldModel.DateTimeType))
					{
						searchFields += "\t\t\tAddSearchField(new SearchField(\"" + field.Name + "\", \"" + field.DisplayName + "\", SearchField.Comparators.Range));\r\n";
					}
					else
					{
						searchFields += "\t\t\tAddSearchField(new SearchField(\"" + field.Name + "\", \"" + field.DisplayName + "\", SearchField.Comparators.ContainsAny));\r\n";
					}
					sortFields += "\t\t\tAddSortField(new SortField(\"" + field.Name + "\", \"" + field.DisplayName + "\"" + (item.i == 1 ? ", true" : "") + "));\r\n";
				}


				//FormFields
				if (field.IsPrimaryKey)
				{
					formFields += "\t\t\tFields.Add(new IdField() { Name = \"" + field.Name + "\" });\r\n";
				}
				else if (field.DataType.Equals(AppFieldModel.DateTimeNullableType) || field.DataType.Equals(AppFieldModel.DateTimeType))
				{
					formFields += "\t\t\tFields.Add(new DateTimeField() { Name = \"" + field.Name + "\" });\r\n";
				}
				else if (field.DataType.Equals(AppFieldModel.intType)
					|| field.DataType.Equals(AppFieldModel.intNullableType)
					|| field.DataType.Equals(AppFieldModel.Int16NullableType)
					|| field.DataType.Equals(AppFieldModel.Int16Type)
					|| field.DataType.Equals(AppFieldModel.Int64NullableType)
					|| field.DataType.Equals(AppFieldModel.Int64Type)
				)
				{
					formFields += "\t\t\tFields.Add(new IntegerField() { Name = \"" + field.Name + "\" });\r\n";
				}
				else if (field.DataType.Equals(AppFieldModel.decimalNullableType) || field.DataType.Equals(AppFieldModel.decimalType) 
						|| field.DataType.Equals(AppFieldModel.doubleNullableType) || field.DataType.Equals(AppFieldModel.doubleType))
				{
					formFields += "\t\t\tFields.Add(new DoubleField() { Name = \"" + field.Name + "\" });\r\n";
				}
				else if (field.DataType.Equals(AppFieldModel.boolType))
				{
					formFields += "\t\t\tFields.Add(new BooleanField() { Name = \"" + field.Name + "\" });\r\n";
				}
				else
				{
					formFields += "\t\t\tFields.Add(new CharField() { Name = \"" + field.Name + "\" });\r\n";
				}

				//IndexColumns
				if (!field.IsPrimaryKey)
				{
					indexPageHeaders += "\t\t\t\t<th>\r\n\t\t\t\t\t" + (field.DisplayName != null ? field.DisplayName : TextService.Title(field.Name)) + "\r\n\t\t\t\t</th>\r\n";
					if (field.DataType.Equals(AppFieldModel.DateTimeNullableType) || field.DataType.Equals(AppFieldModel.DateTimeType))
					{
						indexPageColumns += "\t\t\t<td>\r\n\t\t\t\t{{ a." + field.Name + " | date : 'shortDate' }}\r\n\t\t\t</td>\r\n";
					}
					else if (field.DataType.Equals(AppFieldModel.boolType))
					{
						indexPageColumns += "\t\t\t<td ng-bind-html=\"a." + field.Name + " | check \">\r\n\t\t\t</td>\r\n";
					}
					else if (field.DataType.Equals(AppFieldModel.intType)
						|| field.DataType.Equals(AppFieldModel.intNullableType)
						|| field.DataType.Equals(AppFieldModel.Int16NullableType)
						|| field.DataType.Equals(AppFieldModel.Int16Type)
						|| field.DataType.Equals(AppFieldModel.Int64NullableType)
						|| field.DataType.Equals(AppFieldModel.Int64Type))
					{
						indexPageColumns += "\t\t\t<td>\r\n\t\t\t\t{{ a." + field.Name + " | number }}\r\n\t\t\t</td>\r\n";
					}
					else
					{
						indexPageColumns += "\t\t\t<td>\r\n\t\t\t\t{{ a." + field.Name + " }}\r\n\t\t\t</td>\r\n";
					}

				}

				//SQL
				if (field.DataType.Equals(AppFieldModel.stringType))
				{
					sqlColumns += "\t[" + (!string.IsNullOrWhiteSpace(field.ColumnName) ? field.ColumnName : field.Name) +
						"] [nvarchar](" + ((field.StringLength != null && field.StringLength > 0) ? field.StringLength.ToString() : "MAX") + ") " +
						(field.IsPrimaryKey ? "NOT NULL" : "NULL") + ",\r\n";    
				}
				else if (field.DataType.Equals(AppFieldModel.intType)
						|| field.DataType.Equals(AppFieldModel.Int16Type)
						|| field.DataType.Equals(AppFieldModel.Int64Type))
				{
					sqlColumns += "[" + (!string.IsNullOrWhiteSpace(field.ColumnName) ? field.ColumnName : field.Name) +
						"] [" + ints[field.DataType] +"] NOT NULL,\r\n";
				}
				else if (field.DataType.Equals(AppFieldModel.intNullableType)
						|| field.DataType.Equals(AppFieldModel.Int16NullableType)
						|| field.DataType.Equals(AppFieldModel.Int64NullableType))
				{
					sqlColumns += "\t[" + (!string.IsNullOrWhiteSpace(field.ColumnName) ? field.ColumnName : field.Name) +
						"] [" + ints[field.DataType] + "] NOT NULL,\r\n";
				}
				else if (field.DataType.Equals(AppFieldModel.DateTimeType))
				{
					sqlColumns += "\t[" + (!string.IsNullOrWhiteSpace(field.ColumnName) ? field.ColumnName : field.Name) +
						"] [datetime] NOT NULL,\r\n";
				}
				else if (field.DataType.Equals(AppFieldModel.DateTimeNullableType))
				{
					sqlColumns += "\t[" + (!string.IsNullOrWhiteSpace(field.ColumnName) ? field.ColumnName : field.Name) +
						"] [datetime] NULL,\r\n";
				}
				else if (field.DataType.Equals(AppFieldModel.boolType))
				{
					sqlColumns += "\t[" + (!string.IsNullOrWhiteSpace(field.ColumnName) ? field.ColumnName : field.Name) +
						"] [bit] NULL,\r\n";
				}
				else if (field.DataType.Equals(AppFieldModel.decimalType))
				{
					sqlColumns += "\t[" + (!string.IsNullOrWhiteSpace(field.ColumnName) ? field.ColumnName : field.Name) +
						"] [decimal](18,2) NOT NULL,\r\n";
				}
				else if (field.DataType.Equals(AppFieldModel.decimalNullableType))
				{
					sqlColumns += "\t[" + (!string.IsNullOrWhiteSpace(field.ColumnName) ? field.ColumnName : field.Name) +
						"] [decimal](18,2) NULL,\r\n";
				}
				else if (field.DataType.Equals(AppFieldModel.doubleType))
				{
					sqlColumns += "\t[" + (!string.IsNullOrWhiteSpace(field.ColumnName) ? field.ColumnName : field.Name) +
						"] [float] NOT NULL,\r\n";
				}
				else if (field.DataType.Equals(AppFieldModel.doubleNullableType))
				{
					sqlColumns += "\t[" + (!string.IsNullOrWhiteSpace(field.ColumnName) ? field.ColumnName : field.Name) +
						"] [float] NULL,\r\n";
				}
				else if (field.DataType.Equals(AppFieldModel.guidType))
				{
					sqlColumns += "\t[" + (!string.IsNullOrWhiteSpace(field.ColumnName) ? field.ColumnName : field.Name) +
						"] [uniqueidentifier] NOT NULL,\r\n";
				}
				else if (field.DataType.Equals(AppFieldModel.guidNullableType))
				{
					sqlColumns += "\t[" + (!string.IsNullOrWhiteSpace(field.ColumnName) ? field.ColumnName : field.Name) +
						"] [uniqueidentifier] NULL,\r\n";
				}
				else if (field.DataType.Equals(AppFieldModel.DbGeographyType))
				{
					sqlColumns += "\t[" + (!string.IsNullOrWhiteSpace(field.ColumnName) ? field.ColumnName : field.Name) +
						"] [DbGeography] NOT NULL,\r\n";
				}

				else if (field.DataType.Equals(AppFieldModel.DbGeographyNullableType))
				{
					sqlColumns += "\t[" + (!string.IsNullOrWhiteSpace(field.ColumnName) ? field.ColumnName : field.Name) +
						"] [DbGeography] NULL,\r\n";
				}


				if (field.IsPrimaryKey)
				{
					sqlKeyList.Add("[" + (!string.IsNullOrWhiteSpace(field.ColumnName) ? field.ColumnName : field.Name) + "] ASC");
				}
			   
			}

			if (model.InheritsBaseModel)
			{
				sqlColumns += @"    [ModifiedById] [nvarchar](128) NULL,
	[Modified][datetime] NULL,
	[CreatedById] [nvarchar](128) NULL,
	[Created] [datetime] NULL,";
			}
			string modelUsings = string.Join("\r\n\t\t", modelUsingList);
			string baseModel = (model.InheritsBaseModel ? " : BaseModel" : "");
			string expands = "\t\t\t" + (expandList.Count > 0 ? "Expand = \"" + string.Join(",", expandList) + "\";" : "");
			string sqlKeys = string.Join(", ", sqlKeyList);
			while (fileQueue.Count > 0)
			{
				string file = fileQueue.Dequeue();
				string filePath = file.Replace(TemplatePath, AreaPath)
					.Replace("_Template", model.Name)
					.Replace(".cst", ".cs")
					.Replace(".cshtmlt", ".cshtml")
					.Replace(".configt", ".config");
				if (!Directory.Exists(filePath))
				{
					string content = System.IO.File.ReadAllText(file);
					content = content.Replace("[Area]", model.Area)
						.Replace("[Name]", model.Name)
						.Replace("[NamePlural]", model.NamePlural)
						.Replace("[Title]", model.Title)
						.Replace("[TitlePlural]", model.TitlePlural)
						.Replace("[TableName]", model.TableName)
						.Replace("[ModelFields]", modelFields)
						.Replace("[ModelUsings]", modelUsings)
						.Replace("[SearchFields]", searchFields)
						.Replace("[SortFields]", sortFields)
						.Replace("[FormFields]", formFields)
						.Replace("[BaseModel]", baseModel)
						.Replace("[IndexPageHeaders]", indexPageHeaders)
						.Replace("[IndexPageColumns]", indexPageColumns)
						.Replace("[Expands]", expands)
						.Replace("[COLUMNS]", sqlColumns)
						.Replace("[TABLENAME]", model.TableName)
						.Replace("[COLUMNKEYS]", sqlKeys);

					System.IO.File.WriteAllText(filePath, content);
				}
			}
			return true;
		}
	}
}