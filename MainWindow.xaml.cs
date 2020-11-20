﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoLotModel1;

namespace Gradinaru_Alexandra_Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerViewSource;
        CollectionViewSource customerOrdersViewSource;
        CollectionViewSource inventoryViewSource;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Data.CollectionViewSource customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // customerViewSource.Source = [generic data source]
            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerViewSource.Source = ctx.Customers.Local;
            customerOrdersViewSource =((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));
            //customerOrdersViewSource.Source = ctx.Orders.Local;
            ctx.Customers.Load();
            // ctx.Orders.Load();
            System.Windows.Data.CollectionViewSource inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // inventoryViewSource.Source = [generic data source]
            ctx.Inventories.Load();
            cmbCustomers.ItemsSource = ctx.Customers.Local;
            //cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustId";
            cmbInventory.ItemsSource = ctx.Inventories.Local;
            //cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.SelectedValuePath = "CarId";
            BindDataGrid();
            inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            inventoryViewSource.Source = ctx.Inventories.Local;
        }


        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals
                             cust.CustId
                             join inv in ctx.Inventories on ord.CarId
                 equals inv.CarId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CarId,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 inv.Make,
                                 inv.Color
                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }

        private void SetValidationBinding()
        {
            Binding firstNameValidationBinding = new Binding();
            firstNameValidationBinding.Source = customerViewSource;
            firstNameValidationBinding.Path = new PropertyPath("FirstName");
            firstNameValidationBinding.NotifyOnValidationError = true;
            firstNameValidationBinding.Mode = BindingMode.TwoWay;
            firstNameValidationBinding.UpdateSourceTrigger =
           UpdateSourceTrigger.PropertyChanged;
            //string required
            firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
            firstNameTextBox.SetBinding(TextBox.TextProperty,
           firstNameValidationBinding);
            Binding lastNameValidationBinding = new Binding();
            lastNameValidationBinding.Source = customerViewSource;
            lastNameValidationBinding.Path = new PropertyPath("LastName");
            lastNameValidationBinding.NotifyOnValidationError = true;
            lastNameValidationBinding.Mode = BindingMode.TwoWay;
            lastNameValidationBinding.UpdateSourceTrigger =
           UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            lastNameTextBox.SetBinding(TextBox.TextProperty,
           lastNameValidationBinding); //setare binding nou
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            customerDataGrid.IsEnabled = false;

            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

            //firstNameTextBox.Text = "";
            firstNameTextBox.IsEnabled = true;
            //lastNameTextBox.Text = "";
            lastNameTextBox.IsEnabled = true;
            Keyboard.Focus(firstNameTextBox);

            customerDataGrid.SelectedItem = null;

            SetValidationBinding();

            //

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            BindingOperations.ClearBinding(custIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);

            customerDataGrid.IsEnabled = false;
            firstNameTextBox.IsEnabled = true;
            lastNameTextBox.IsEnabled = true;
            //custIdTextBox.Text = tempId;
            //firstNameTextBox.Text = tempFirstName;
            //lastNameTextBox.Text = tempLastName;
            Keyboard.Focus(firstNameTextBox);

            SetValidationBinding();
            //
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            customerDataGrid.IsEnabled = false;
            BindingOperations.ClearBinding(custIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
            //
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    customer = new Customer()
                    {
                        FirstName = firstNameTextBox.Text.Trim(),
                        LastName = lastNameTextBox.Text.Trim()
                    };

                    //adaugam entitatea nou creata in context
                    ctx.Customers.Add(customer);
                    customerViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                customerDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                //
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    customer.FirstName = firstNameTextBox.Text.Trim();
                    customer.LastName = lastNameTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                // pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(customer);
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                customerDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                //
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    customer = (Customer)customerDataGrid.SelectedItem;
                    ctx.Customers.Remove(customer);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                customerViewSource.View.Refresh();
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                customerDataGrid.IsEnabled = true;
                firstNameTextBox.IsEnabled = false;
                lastNameTextBox.IsEnabled = false;
                //
            }
            SetValidationBinding();

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnDelete.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;
            customerDataGrid.IsEnabled = true;
            firstNameTextBox.IsEnabled = false;
            lastNameTextBox.IsEnabled = false;
            //
        }
        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            customerViewSource.View.MoveCurrentToNext();
        }



        private void btnNewInventory_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNewInventory.IsEnabled = false;
            btnEditInventory.IsEnabled = false;
            btnDeleteInventory.IsEnabled = false;
            btnPrevInventory.IsEnabled = false;
            btnNextInventory.IsEnabled = false;
            btnSaveInventory.IsEnabled = true;
            btnCancelInventory.IsEnabled = true;
            inventoryDataGrid.IsEnabled = false;
            BindingOperations.ClearBinding(carIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            makeTextBox.Text = "";
            makeTextBox.IsEnabled = true;
            colorTextBox.Text = "";
            colorTextBox.IsEnabled = true;
            Keyboard.Focus(colorTextBox);
            //
        }

        private void btnEditInventory_Click(object sender, RoutedEventArgs e)
        {
            //
            action = ActionState.Edit;
            btnNewInventory.IsEnabled = false;
            btnEditInventory.IsEnabled = false;
            btnDeleteInventory.IsEnabled = false;
            btnPrevInventory.IsEnabled = false;
            btnNextInventory.IsEnabled = false;
            btnSaveInventory.IsEnabled = true;
            btnCancelInventory.IsEnabled = true;
            inventoryDataGrid.IsEnabled = false;
            BindingOperations.ClearBinding(carIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
            makeTextBox.IsEnabled = true;
            colorTextBox.IsEnabled = true;
            Keyboard.Focus(colorTextBox);

            //
        }

        private void btnDeleteInventory_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            btnNewInventory.IsEnabled = false;
            btnEditInventory.IsEnabled = false;
            btnDeleteInventory.IsEnabled = false;
            btnPrevInventory.IsEnabled = false;
            btnNextInventory.IsEnabled = false;
            btnSaveInventory.IsEnabled = true;
            btnCancelInventory.IsEnabled = true;
            inventoryDataGrid.IsEnabled = false;
            BindingOperations.ClearBinding(carIdTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(makeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(colorTextBox, TextBox.TextProperty);
           
            //
        }

        private void btnSaveInventory_Click(object sender, RoutedEventArgs e)
        {
                Inventory inventory = null;
                if (action == ActionState.New)
                {
                    try
                    {
                        //instantiem Inventory entity
                        inventory = new Inventory()
                        {
                            Make = makeTextBox.Text.Trim(),
                            Color = colorTextBox.Text.Trim()
                        };
                        //adaugam entitatea nou creata in context
                        ctx.Inventories.Add(inventory);
                        inventoryViewSource.View.Refresh();
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                    //using System.Data;
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    btnNewInventory.IsEnabled = true;
                    btnEditInventory.IsEnabled = true;
                    btnDeleteInventory.IsEnabled = true;
                    btnSaveInventory.IsEnabled = false;
                    btnCancelInventory.IsEnabled = false;
                    btnPrevInventory.IsEnabled = true;
                    btnNextInventory.IsEnabled = true;
                    inventoryDataGrid.IsEnabled = true;
                    makeTextBox.IsEnabled = false;
                    colorTextBox.IsEnabled = false;
            }
                else if (action == ActionState.Edit)
                {
                    try
                    {
                        inventory = (Inventory)inventoryDataGrid.SelectedItem;
                        inventory.Make = makeTextBox.Text.Trim();
                        inventory.Color = colorTextBox.Text.Trim();
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    inventoryViewSource.View.Refresh();
                    // pozitionarea pe item-ul curent
                    inventoryViewSource.View.MoveCurrentTo(inventory);
                    btnNewInventory.IsEnabled = true;
                    btnEditInventory.IsEnabled = true;
                    btnDeleteInventory.IsEnabled = true;
                    btnSaveInventory.IsEnabled = false;
                    btnCancelInventory.IsEnabled = false;
                    btnPrevInventory.IsEnabled = true;
                    btnNextInventory.IsEnabled = true;
                    inventoryDataGrid.IsEnabled = true;
                    makeTextBox.IsEnabled = false;
                    colorTextBox.IsEnabled = false;
            }
                else if (action == ActionState.Delete)
                {
                    try
                    {
                        inventory = (Inventory)inventoryDataGrid.SelectedItem;
                        ctx.Inventories.Remove(inventory);
                        ctx.SaveChanges();
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    inventoryViewSource.View.Refresh();
                    btnNewInventory.IsEnabled = true;
                    btnEditInventory.IsEnabled = true;
                    btnDeleteInventory.IsEnabled = true;
                    btnSaveInventory.IsEnabled = false;
                    btnCancelInventory.IsEnabled = false;
                    btnPrevInventory.IsEnabled = true;
                    btnNextInventory.IsEnabled = true;
                    inventoryDataGrid.IsEnabled = true;
                    makeTextBox.IsEnabled = false;
                    colorTextBox.IsEnabled = false;
            }
            }

            private void btnCancelInventory_Click(object sender, RoutedEventArgs e)
            {
                action = ActionState.Nothing;
                btnNewInventory.IsEnabled = true;
                btnEditInventory.IsEnabled = true;
                btnDeleteInventory.IsEnabled = true;
                btnSaveInventory.IsEnabled = false;
                btnCancelInventory.IsEnabled = false;
                btnPrevInventory.IsEnabled = true;
                btnNextInventory.IsEnabled = true;
                inventoryDataGrid.IsEnabled = true;
                makeTextBox.IsEnabled = false;
                colorTextBox.IsEnabled = false;
        }

            private void btnPrevInventory_Click(object sender, RoutedEventArgs e)
            {
                inventoryViewSource.View.MoveCurrentToPrevious();
            }

            private void btnNextInventory_Click(object sender, RoutedEventArgs e)
            {
                inventoryViewSource.View.MoveCurrentToNext();
            }

        private void btnNewOrder_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNewOrder.IsEnabled = false;
            btnDeleteOrder.IsEnabled = false;
            btnEditOrder.IsEnabled = false;
            btnPrevOrder.IsEnabled = false;
            btnNextOrder.IsEnabled = false;
        }

        private void btnEditOrder_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;
            btnNewOrder.IsEnabled = false;
            btnDeleteOrder.IsEnabled = false;
            btnEditOrder.IsEnabled = false;
            btnPrevOrder.IsEnabled = false;
            btnNextOrder.IsEnabled = false;
        }

        private void btnDeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;
            btnNewOrder.IsEnabled = false;
            btnDeleteOrder.IsEnabled = false;
            btnEditOrder.IsEnabled = false;
            btnPrevOrder.IsEnabled = false;
            btnNextOrder.IsEnabled = false;
        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Customer customer = (Customer)cmbCustomers.SelectedItem;
                    Inventory inventory = (Inventory)cmbInventory.SelectedItem;
                    //instantiem Order entity
                    order = new Order()
                    {

                        CustId = customer.CustId,
                        CarId = inventory.CarId
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Orders.Add(order);
                    customerOrdersViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            if (action == ActionState.Edit)
            {
                dynamic selectedOrder = ordersDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;
                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                        editedOrder.CarId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                // pozitionarea pe item-ul curent
                customerViewSource.View.MoveCurrentTo(selectedOrder);
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = ordersDataGrid.SelectedItem;
                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            btnNewOrder.IsEnabled = true;
            btnDeleteOrder.IsEnabled = true;
            btnEditOrder.IsEnabled = true;
            btnPrevOrder.IsEnabled = true;
            btnNextOrder.IsEnabled = true;
        }

        private void btnCancelOrder_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNewOrder.IsEnabled = true;
            btnEditOrder.IsEnabled = true;
            btnDeleteOrder.IsEnabled = true;
            btnSaveOrder.IsEnabled = false;
            btnCancelOrder.IsEnabled = false;
            btnPrevOrder.IsEnabled = true;
            btnNextOrder.IsEnabled = true;
        }

        private void btnPrevOrder_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNextOrder_Click(object sender, RoutedEventArgs e)
        {
            customerOrdersViewSource.View.MoveCurrentToNext();
        }
    }
   }
