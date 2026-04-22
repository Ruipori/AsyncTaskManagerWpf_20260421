using System;
using System.Collections.Generic;
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

using AsyncTaskManagerWpf.Models;
using AsyncTaskManagerWpf.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AsyncTaskManagerWpf
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly TodoRepository _repository = new TodoRepository();

        private string _newTitle = "";
        private string _newDescription = "";
        private TodoItem _selectedTodo;

        private string _searchText = "";
        private ICollectionView _filteredTodos;

        private string _currentFilter = "All";

        public ObservableCollection<TodoItem> Todos { get; set; } = new ObservableCollection<TodoItem>();

        public string NewTitle {
            get => _newTitle;
            set {
                _newTitle = value;
                OnPropertyChanged();
            }
        }

        public string NewDescription {
            get => _newDescription;
            set {
                _newDescription = value;
                OnPropertyChanged();
            }
        }

        public TodoItem SelectedTodo {
            get => _selectedTodo;
            set {
                _selectedTodo = value;
                OnPropertyChanged();
            }
        }
        public ICollectionView FilteredTodos {
            get { return _filteredTodos; }
            set {
                _filteredTodos = value;
                OnPropertyChanged();
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged();

                if ( FilteredTodos != null )
                {
                    FilteredTodos.Refresh();
                }
            }
        }

        public string TodayText
        {
            get { return DateTime.Now.ToString( "M월 d일, dddd" ); }
        }

        public string ProgressText
        {
            get {
                if ( Todos.Count == 0 )
                    return "0%";

                double percent = ( double )Todos.Count( t => t.IsCompleted ) / Todos.Count * 100;
                return ( ( int )percent ).ToString() + "%";
            }
        }

        public string ProgressDetailText {
            get {
                return $"{Todos.Count( t => t.IsCompleted )} / {Todos.Count} 완료됨";
            }
        }

        public string CurrentFilterText {
            get {
                if ( _currentFilter == "Active" ) return "진행 중";
                if ( _currentFilter == "Completed" ) return "완료됨";
                return "전체 작업";
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            FilteredTodos = CollectionViewSource.GetDefaultView( Todos );
            FilteredTodos.Filter = FilterTodo;

            Loaded += MainWindow_Loaded;

            _currentFilter = "All";
        }
        private bool FilterTodo( object obj )
        {
            #region 기존
            //TodoItem todo = obj as TodoItem;

            //if ( todo == null )
            //    return false;

            //if ( string.IsNullOrWhiteSpace( SearchText ) )
            //    return true;

            //string keyword = SearchText.Trim().ToLower();

            //return ( todo.Title != null && todo.Title.ToLower().Contains( keyword ) )
            //       || ( todo.Description != null && todo.Description.ToLower().Contains( keyword ) );
            #endregion
            TodoItem todo = obj as TodoItem;

            if ( todo == null )
                return false;

            // 🔍 검색 필터
            if ( !string.IsNullOrWhiteSpace( SearchText ) ) {
                string keyword = SearchText.Trim().ToLower();

                string title = todo.Title == null ? "" : todo.Title.ToLower();
                string description = todo.Description == null ? "" : todo.Description.ToLower();

                if ( !title.Contains( keyword ) && !description.Contains( keyword ) )
                    return false;
            }

            // 📊 상태 필터
            if ( _currentFilter == "Active" && todo.IsCompleted )
                return false;

            if ( _currentFilter == "Completed" && !todo.IsCompleted )
                return false;

            return true;
        }

        private void RefreshDashboard()
        {
            OnPropertyChanged( nameof( ProgressText ) );
            OnPropertyChanged( nameof( ProgressDetailText ) );
            OnPropertyChanged( nameof( TodayText ) );

            if ( FilteredTodos != null ) {
                FilteredTodos.Refresh();
            }
        }

        private async void MainWindow_Loaded( object sender, RoutedEventArgs e )
        {
            await LoadTodosAsync();
            UpdateSidebarFilterButtons();
        }

        private async void AddButton_Click( object sender, RoutedEventArgs e )
        {
            //if ( string.IsNullOrWhiteSpace( NewTitle ) ) {
            //    MessageBox.Show( "제목을 입력해주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Information );
            //    return;
            //}
            if ( string.IsNullOrWhiteSpace( NewTitle ) )
            {
                new CustomMessageBox( "제목을 입력해주세요.", "알림" )
                {
                    Owner = this
                }.ShowDialog();
                return;
            }
            int nextId = Todos.Any() ? Todos.Max( t => t.Id ) + 1 : 1;

            var todo = new TodoItem
            {
                Id = nextId,
                Title = NewTitle.Trim(),
                Description = NewDescription.Trim(),
                IsCompleted = false,
                CreatedAt = System.DateTime.Now
            };

            Todos.Add( todo );

            NewTitle = "";
            NewDescription = "";
            RefreshDashboard();
            await SaveTodosAsync();
        }

        private async void DeleteButton_Click( object sender, RoutedEventArgs e )
        {
            //if ( SelectedTodo == null ) {
            //    MessageBox.Show( "삭제할 항목을 선택해주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Information );
            //    return;
            //}

            //var result = MessageBox.Show(
            //    $"'{SelectedTodo.Title}' 항목을 삭제할까요?",
            //    "삭제 확인",
            //    MessageBoxButton.YesNo,
            //    MessageBoxImage.Question );

            //if ( result != MessageBoxResult.Yes )
            //    return;

            //Todos.Remove( SelectedTodo );
            //SelectedTodo = null;
            //RefreshDashboard();
            //await SaveTodosAsync();
            if ( SelectedTodo == null )
            {
                new CustomMessageBox( "삭제할 항목을 선택해주세요.", "알림" )
                {
                    Owner = this
                }.ShowDialog();
                return;
            }

            var msg = new CustomMessageBox( "정말 삭제하시겠습니까?", "삭제", true );
            msg.Owner = this;
            msg.ShowDialog();

            if ( !msg.Result )
                return;

            Todos.Remove( SelectedTodo );

            await SaveTodosAsync();

            new CustomMessageBox( "삭제되었습니다.", "완료" )
            {
                Owner = this
            }.ShowDialog();
        }
        private void ClearSearch_Click( object sender, RoutedEventArgs e )
        {
            SearchText = "";
        }
        private async void ToggleCompleteButton_Click( object sender, RoutedEventArgs e )
        {
            //if ( SelectedTodo == null ) {
            //    MessageBox.Show( "완료 상태를 바꿀 항목 선택.", "알림", MessageBoxButton.OK, MessageBoxImage.Information );
            //    return;
            //}
            if ( SelectedTodo == null ) {
                new CustomMessageBox( "완료 상태를 바꿀 항목 선택해주세요.", "알림" )
                {
                    Owner = this
                }.ShowDialog();
                return;
            }
            SelectedTodo.IsCompleted = !SelectedTodo.IsCompleted;
            RefreshDashboard();
            await SaveTodosAsync();
        }

        private async void SaveButton_Click( object sender, RoutedEventArgs e )
        {
            //await SaveTodosAsync();
            //MessageBox.Show( "저장이 완료되었습니다.", "저장", MessageBoxButton.OK, MessageBoxImage.Information );
            var msg = new CustomMessageBox( "저장하시겠습니까?", "저장", true );
            msg.Owner = this;
            msg.ShowDialog();

            if ( !msg.Result )
                return;

            await SaveTodosAsync();

            new CustomMessageBox( "저장이 완료되었습니다.", "완료" )
            {
                Owner = this
            }.ShowDialog();
        }

        private async void LoadButton_Click( object sender, RoutedEventArgs e )
        {
            await LoadTodosAsync();
            //MessageBox.Show( "불러오기가 완료되었습니다.", "불러오기", MessageBoxButton.OK, MessageBoxImage.Information );
            new CustomMessageBox( "불러오기가 완료되었습니다.", "불러오기" )
            {
                Owner = this
            }.ShowDialog();
        }

        private void AllFilter_Click( object sender, RoutedEventArgs e )
        {
            _currentFilter = "All";
            UpdateSidebarFilterButtons();
            RefreshDashboard();
        }

        private void ActiveFilter_Click( object sender, RoutedEventArgs e )
        {
            _currentFilter = "Active";
            UpdateSidebarFilterButtons();
            RefreshDashboard();
        }

        private void CompletedFilter_Click( object sender, RoutedEventArgs e )
        {
            _currentFilter = "Completed";
            UpdateSidebarFilterButtons();
            RefreshDashboard();
        }
        private void TitleBar_MouseDown( object sender, MouseButtonEventArgs e )
        {
            if ( e.ButtonState == MouseButtonState.Pressed ) {
                DragMove();
            }
        }
        private void Minimize_Click( object sender, RoutedEventArgs e )
        {
            WindowState = WindowState.Minimized;
        }
        private void Close_Click( object sender, RoutedEventArgs e )
        {
            Close();
        }
        private void Maximize_Click( object sender, RoutedEventArgs e )
        {
            if ( WindowState == WindowState.Maximized )
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }
        private async System.Threading.Tasks.Task SaveTodosAsync()
        {
            try
            {
                await _repository.SaveAsync( Todos.ToList() );
            }
            catch ( System.Exception ex )
            {
                //MessageBox.Show( $"저장 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error );
                new CustomMessageBox( $"저장 중 오류가 발생했습니다.\n{ex.Message}", "오류" )
                {
                    Owner = this
                }.ShowDialog();
                //CustomMessageBox.ShowConfirm( $"저장 중 오류가 발생했습니다.\n{ex.Message}", "오류", CustomMessageBoxType.Error );
            }
        }

        private async System.Threading.Tasks.Task LoadTodosAsync()
        {
            try {
                var items = await _repository.LoadAsync();

                Todos.Clear();
                foreach ( var item in items ) {
                    Todos.Add( item );
                }
                RefreshDashboard();
            }
            catch ( System.Exception ex )
            {
                //MessageBox.Show( $"불러오기 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error );
                new CustomMessageBox( $"불러오기 중 오류가 발생했습니다.\n{ex.Message}", "오류" )
                {
                    Owner = this
                }.ShowDialog();
                //CustomMessageBox.ShowConfirm( $"불러오기 중 오류가 발생했습니다.\n{ex.Message}", "오류", CustomMessageBoxType.Error );
            }
        }
        private void UpdateSidebarFilterButtons()
        {
            var selectedBrush = FindResource( "PurpleButtonBrush" ) as System.Windows.Media.Brush;
            var selectedForeground = System.Windows.Media.Brushes.White;
            var normalBackground = System.Windows.Media.Brushes.White;
            var normalForeground = ( System.Windows.Media.Brush )new System.Windows.Media.BrushConverter().ConvertFromString( "#403A8B" );

            AllFilterButton.Background = normalBackground;
            AllFilterButton.Foreground = normalForeground;

            ActiveFilterButton.Background = normalBackground;
            ActiveFilterButton.Foreground = normalForeground;

            CompletedFilterButton.Background = normalBackground;
            CompletedFilterButton.Foreground = normalForeground;

            if ( _currentFilter == "All" ) {
                AllFilterButton.Background = selectedBrush;
                AllFilterButton.Foreground = selectedForeground;
            } else if ( _currentFilter == "Active" ) {
                ActiveFilterButton.Background = selectedBrush;
                ActiveFilterButton.Foreground = selectedForeground;
            } else if ( _currentFilter == "Completed" ) {
                CompletedFilterButton.Background = selectedBrush;
                CompletedFilterButton.Foreground = selectedForeground;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged( [CallerMemberName] string propertyName = null )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }


    }
}
