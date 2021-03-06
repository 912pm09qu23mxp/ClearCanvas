#region License

// Copyright (c) 2013, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This file is part of the ClearCanvas RIS/PACS open source project.
//
// The ClearCanvas RIS/PACS open source project is free software: you can
// redistribute it and/or modify it under the terms of the GNU General Public
// License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// The ClearCanvas RIS/PACS open source project is distributed in the hope that it
// will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// the ClearCanvas RIS/PACS open source project.  If not, see
// <http://www.gnu.org/licenses/>.

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	/// <summary>
	/// Extension point for views onto <see cref="LinkProceduresComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class LinkProceduresComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// LinkProceduresComponent class
	/// </summary>
	[AssociateView(typeof(LinkProceduresComponentViewExtensionPoint))]
	public class LinkProceduresComponent : ApplicationComponent
	{
		private Table<Checkable<ReportingWorklistItemSummary>> _candidateTable;
		private readonly List<ReportingWorklistItemSummary> _candidates;

		private readonly ReportingWorklistItemSummary _sourceItem;
		private ReportingWorklistTable _sourceTable;

		private readonly string _instructions;
		private readonly string _heading;

		/// <summary>
		/// Constructor
		/// </summary>
		public LinkProceduresComponent(ReportingWorklistItemSummary sourceItem, List<ReportingWorklistItemSummary> candidateItems, string instructions, string heading)
		{
			_candidates = candidateItems;
			_sourceItem = sourceItem;
			_instructions = instructions;
			_heading = heading;
		}

		public LinkProceduresComponent(ReportingWorklistItemSummary sourceItem, List<ReportingWorklistItemSummary> candidateItems)
			: this(sourceItem, candidateItems, SR.TextLinkReportInstructions, SR.TextLinkReportlHeading)
		{
		}

		public override void Start()
		{
			_candidateTable = new Table<Checkable<ReportingWorklistItemSummary>>();
			_candidateTable.Columns.Add(new TableColumn<Checkable<ReportingWorklistItemSummary>, bool>(".",
				delegate(Checkable<ReportingWorklistItemSummary> item) { return item.IsChecked; },
				delegate(Checkable<ReportingWorklistItemSummary> item, bool value) { item.IsChecked = value; }, 0.20f));
			_candidateTable.Columns.Add(new TableColumn<Checkable<ReportingWorklistItemSummary>, string>(SR.ColumnProcedure,
				delegate(Checkable<ReportingWorklistItemSummary> item) { return item.Item.ProcedureName; }, 2.75f));
			_candidateTable.Columns.Add(new DateTimeTableColumn<Checkable<ReportingWorklistItemSummary>>(SR.ColumnTime,
				delegate(Checkable<ReportingWorklistItemSummary> item) { return item.Item.Time; }, 0.5f));

			foreach (ReportingWorklistItemSummary item in _candidates)
			{
				_candidateTable.Items.Add(new Checkable<ReportingWorklistItemSummary>(item, true));
			}

			_sourceTable = new ReportingWorklistTable();
			_sourceTable.Items.Add(_sourceItem);

			base.Start();
		}

		public List<ReportingWorklistItemSummary> SelectedItems
		{
			get
			{
				return CollectionUtils.Map<Checkable<ReportingWorklistItemSummary>, ReportingWorklistItemSummary>(
					CollectionUtils.Select(_candidateTable.Items,
						delegate(Checkable<ReportingWorklistItemSummary> item) { return item.IsChecked; }),
							delegate(Checkable<ReportingWorklistItemSummary> checkableItem) { return checkableItem.Item; });
			}
		}

		#region Presentation Model

		public ITable SourceTable
		{
			get { return _sourceTable; }
		}

		public ITable CandidateTable
		{
			get { return _candidateTable; }
		}

		public string Instructions
		{
			get { return _instructions; }
		}

		public string Heading
		{
			get { return _heading; }
		}

		public void Accept()
		{
			this.Exit(ApplicationComponentExitCode.Accepted);
		}

		#endregion
	}
}
