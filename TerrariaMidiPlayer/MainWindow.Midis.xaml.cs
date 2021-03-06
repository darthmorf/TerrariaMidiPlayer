﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TerrariaMidiPlayer.Util;
using TerrariaMidiPlayer.Windows;

namespace TerrariaMidiPlayer {
	/**<summary>The main window running Terraria Midi Player.</summary>*/
	public partial class MainWindow : Window {
		//============ EVENTS ============
		#region Events

		private void OnMidiChanged(object sender, SelectionChangedEventArgs e) {
			if (!loaded)
				return;

			Stop();
			Config.MidiIndex = listMidis.SelectedIndex;
			UpdateMidi();
		}

		private void OnAddMidi(object sender, RoutedEventArgs e) {
			Stop();
			loaded = false;

			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Midi Files|*.mid;*.midi|All Files|*.*";
			dialog.FilterIndex = 0;
			var result = dialog.ShowDialog(this);
			if (result.HasValue && result.Value) {
				Midi midi = new Midi();
				if (midi.Load(dialog.FileName)) {
					Config.Midis.Add(midi);
					Config.MidiIndex = Config.MidiCount - 1;
					listMidis.Items.Add(midi.ProperName);
					listMidis.SelectedIndex = Config.MidiIndex;
					listMidis.ScrollIntoView(listMidis.Items[Config.MidiIndex]);
					UpdateMidi();
				}
				else {
					var result2 = TriggerMessageBox.Show(this, MessageIcon.Error, "Error when loading the selected midi! Would you like to see the error?", "Load Error", MessageBoxButton.YesNo);
					if (result2 == MessageBoxResult.Yes)
						ErrorMessageBox.Show(midi.LoadException, true);
				}
			}

			loaded = true;
		}

		private void OnRemoveMidi(object sender, RoutedEventArgs e) {
			Stop();
			loaded = false;

			if (Config.HasMidi) {
				var result = TriggerMessageBox.Show(this, MessageIcon.Warning, "Are you sure you want to remove this midi?", "Remove Midi", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes) {
					Config.Midis.RemoveAt(Config.MidiIndex);
					listMidis.Items.RemoveAt(Config.MidiIndex);

					if (Config.MidiIndex > 0)
						Config.MidiIndex--;
					else if (Config.MidiIndex >= Config.MidiCount)
						Config.MidiIndex = Config.MidiCount - 1;

					listMidis.SelectedIndex = Config.MidiIndex;
					if (Config.HasMidi)
						listMidis.ScrollIntoView(listMidis.Items[Config.MidiIndex]);

					UpdateMidi();
				}
			}

			loaded = true;
		}

		private void OnEditMidiName(object sender, RoutedEventArgs e) {
			Stop();
			loaded = false;

			if (Config.HasMidi) {
				string newName = EditNameDialog.ShowDialog(this, Config.Midi.ProperName);
				if (newName != null) {
					Config.Midi.Name = newName;
					listMidis.Items[listMidis.SelectedIndex] = Config.Midi.ProperName;
					listMidis.SelectedIndex = Config.MidiIndex;
				}
			}

			loaded = true;
		}

		private void OnMoveMidiUp(object sender, RoutedEventArgs e) {
			Stop();
			loaded = false;

			if (Config.HasMidi && Config.MidiIndex > 0) {
				Midi midi = Config.Midi;
				Config.Midis.RemoveAt(Config.MidiIndex);
				listMidis.Items.RemoveAt(Config.MidiIndex);

				Config.MidiIndex--;

				Config.Midis.Insert(Config.MidiIndex, midi);
				listMidis.Items.Insert(Config.MidiIndex, midi.ProperName);
				listMidis.SelectedIndex = Config.MidiIndex;
				listMidis.ScrollIntoView(listMidis.Items[Config.MidiIndex]);
				UpdateMidiButtons();
			}

			loaded = true;
		}

		private void OnMoveMidiDown(object sender, RoutedEventArgs e) {
			Stop();
			loaded = false;

			if (Config.HasMidi && Config.MidiIndex + 1 < Config.MidiCount) {
				Midi midi = Config.Midi;
				Config.Midis.RemoveAt(Config.MidiIndex);
				listMidis.Items.RemoveAt(Config.MidiIndex);

				Config.MidiIndex++;

				Config.Midis.Insert(Config.MidiIndex, midi);
				listMidis.Items.Insert(Config.MidiIndex, midi.ProperName);
				listMidis.SelectedIndex = Config.MidiIndex;
				listMidis.ScrollIntoView(listMidis.Items[Config.MidiIndex]);
				UpdateMidiButtons();
			}

			loaded = true;
		}

		#endregion
	}
}
