/* Этот файл является частью примеров использования библиотеки Saraff.Twain.NET
 * © SARAFF SOFTWARE (Кирножицкий Андрей), 2011.
 * Saraff.Twain.NET - свободная программа: вы можете перераспространять ее и/или
 * изменять ее на условиях Меньшей Стандартной общественной лицензии GNU в том виде,
 * в каком она была опубликована Фондом свободного программного обеспечения;
 * либо версии 3 лицензии, либо (по вашему выбору) любой более поздней
 * версии.
 * Saraff.Twain.NET распространяется в надежде, что она будет полезной,
 * но БЕЗО ВСЯКИХ ГАРАНТИЙ; даже без неявной гарантии ТОВАРНОГО ВИДА
 * или ПРИГОДНОСТИ ДЛЯ ОПРЕДЕЛЕННЫХ ЦЕЛЕЙ. Подробнее см. в Меньшей Стандартной
 * общественной лицензии GNU.
 * Вы должны были получить копию Меньшей Стандартной общественной лицензии GNU
 * вместе с этой программой. Если это не так, см.
 * <http://www.gnu.org/licenses/>.)
 * 
 * This file is part of samples of Saraff.Twain.NET.
 * © SARAFF SOFTWARE (Kirnazhytski Andrei), 2011.
 * Saraff.Twain.NET is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * Saraff.Twain.NET is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 * You should have received a copy of the GNU Lesser General Public License
 * along with Saraff.Twain.NET. If not, see <http://www.gnu.org/licenses/>.
 * 
 * PLEASE SEND EMAIL TO:  twain@saraff.ru.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TownSuite.TwainScanner 
{

    internal sealed partial class SelectSourceForm:Form {

        public SelectSourceForm() {
            this.InitializeComponent();
            this.SourceIndex=-1;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            try {
                this.sourceListBox.Items.Clear();
                if(this.Twain!=null&&this.Twain.SourcesCount>0) {
                    for(int i=0; i<this.Twain.SourcesCount; i++) {
                        this.sourceListBox.Items.Add(this.Twain.GetSourceProductName(i));
                    }
                    this.sourceListBox.SelectedIndex=this.Twain.SourceIndex;
                }
            } catch(Exception ex) {
                MessageBox.Show(ex.Message,ex.GetType().Name,MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        public Twain32 Twain {
            get;
            set;
        }

        public int SourceIndex {
            get;
            private set;
        }

        private void selectButton_Click(object sender,EventArgs e) {
            try {
                this.SourceIndex=this.sourceListBox.SelectedIndex;
            } catch(Exception ex) {
                MessageBox.Show(ex.Message,ex.GetType().Name,MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void SelectSourceForm_Load(object sender, EventArgs e)
        {

        }
    }
}
