using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace Items {
    public class Inventory {
        public const int ItemsPerColumn = 6;
        public List<Item> Items { get; } = new(); //不一定每个格子都有东西，没东西的话就是null
        private int CurrentSelected { get; set; } = -1; //选中的数据存储（负责合成等）
        private int CurrentFocus { get; set; } //光标的数据存储

        public void UpdateHotKeys() {
            if (Input.GetKeyDown(KeyCode.Return)) { //处理选择（回车）逻辑
                if (this.CurrentSelected == this.CurrentFocus) //又点一遍？取消选择
                    this.CurrentSelected = -1;
                else if (this.CurrentSelected == -1) { //目前没有选择另一个东西
                    if (this.Items[this.CurrentFocus] != null) //空槽位不能选择
                        this.CurrentSelected = this.CurrentFocus;
                } else {
                    if (this.Items[this.CurrentFocus] == null) { //移动物品
                        this.Items[this.CurrentFocus] = this.Items[this.CurrentSelected];
                        this.Items[this.CurrentSelected] = null;
                        this.CurrentSelected = -1;
                    } else if (RecipeManager.TryCraft(this.Items[this.CurrentFocus], this.Items[this.CurrentSelected], out Item result)) { //检查合成
                        this.Items[this.CurrentFocus] = result;
                        this.Items[this.CurrentSelected] = null;
                    }
                    this.RemoveSuffixNulls();
                }
            } else if (Input.GetKeyDown(KeyCode.UpArrow)) { //处理上下左右的控制逻辑
                if (this.CurrentFocus > 0) this.CurrentFocus--;
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                if (this.CurrentFocus < this.Items.Count) this.CurrentFocus++;
            } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                if (this.CurrentFocus + ItemsPerColumn < this.Items.Count) this.CurrentFocus += ItemsPerColumn;
            } else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                if (this.CurrentFocus > ItemsPerColumn) this.CurrentFocus -= ItemsPerColumn;
            }
        }

        public void InsertItem(Item item) {
            //先检查前面是否有空位
            bool inserted = false;
            for (int i = 0; i < this.Items.Count; i++) {
                if (this.Items[i] == null) {
                    this.Items[i] = item;
                    inserted = true;
                }
            }
            if (!inserted) this.Items.Add(item);
        }

        public bool HasItem(Item item) {
            return this.Items.Any(t => t == item);
        }

        //不能塞入重复物品
        public bool TryInsertItem(Item item) {
            if (this.HasItem(item)) return false;
            this.InsertItem(item);
            return true;
        }

        [CanBeNull]
        public Item GetHoldingItem() {
            return this.Items[this.CurrentFocus];
        }

        private void RemoveSuffixNulls() {
            while (this.Items.Count > 0 && this.Items[^1] == null)
                this.Items.RemoveAt(this.Items.Count - 1);
        }
    }
}