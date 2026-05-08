using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using THUVIENZ.BLL;
using THUVIENZ.Core;
using THUVIENZ.Models;
using THUVIENZ.DAL;
using Microsoft.EntityFrameworkCore;

namespace THUVIENZ.ViewModels
{
    public class ReaderManagementViewModel : ObservableObject
    {
        private readonly LmsDbContext _context;
        private readonly ReaderManagementService _readerService;

        private ObservableCollection<DocGia> _readers = new ObservableCollection<DocGia>();
        public ObservableCollection<DocGia> Readers
        {
            get => _readers;
            set { _readers = value; OnPropertyChanged(); }
        }

        private int _pendingRequestCount;
        public int PendingRequestCount
        {
            get => _pendingRequestCount;
            set { _pendingRequestCount = value; OnPropertyChanged(); }
        }

        public ICommand LoadReadersCommand { get; }
        public ICommand DeleteReaderCommand { get; }

        public ReaderManagementViewModel()
        {
            _context = new LmsDbContext();
            _readerService = new ReaderManagementService();

            LoadReadersCommand = new RelayCommand(async _ => await LoadDataAsync());
            DeleteReaderCommand = new RelayCommand(async param => await ExecuteDeleteAsync(param));

            _ = LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            var readers = await _context.DocGias.ToListAsync();
            Readers = new ObservableCollection<DocGia>(readers);

            PendingRequestCount = await _context.TaiKhoans.CountAsync(t => t.TrangThai == "Pending");
        }

        private async Task ExecuteDeleteAsync(object? param)
        {
            if (param is DocGia reader)
            {
                try
                {
                    await _readerService.DeleteReaderAsync(reader.MaDocGia);
                    await LoadDataAsync();
                }
                catch (Exception ex)
                {
                    // Log error
                }
            }
        }
    }
}
