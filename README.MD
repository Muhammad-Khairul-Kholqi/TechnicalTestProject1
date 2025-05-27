ProductViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace technicalTestProject_1.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string ProductImage { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Price { get; set; }
        public string CreatorId { get; set; } = string.Empty;
        public string CategoryId { get; set; } = string.Empty;
        public string CreatedAt { get; set; } = string.Empty;

        public class CreateProductViewModel
        {
            [Required(ErrorMessage = "All data must be filled in")]
            public string ProductName { get; set; } = string.Empty;
            public string ProductImage { get; set; } = string.Empty;
            public int Price { get; set; }
            public string CategoryId { get; set; } = string.Empty;
        }

        public class UpdateProductViewModel
        {
            [Required(ErrorMessage = "All data must be filled in")]
            public string ProductName { get; set; } = string.Empty;
            public string ProductImage { get; set; } = string.Empty;
            public int Price { get; set; }
            public string CategoryId { get; set; } = string.Empty;
        }
    }
}

Product.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace technicalTestProject_1.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductImage { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string CreatorId { get; set; } = string.Empty;

        [ForeignKey("CreatorId")]
        public virtual ApplicationUser Creator { get; set; } = null!;

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}

ProductController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[Route("/product")]
[Authorize]
public class ProductController : Controller
{
    [HttpGet]
    public IActionResult IndexProduct()
    {
        return View("~/Views/Product/IndexProduct.cshtml");
    }
}

IndexProduct.cshtml
@{
    ViewData["Title"] = "Product";
}

<div class="bg-white p-5 rounded-lg">
    <h1 class="text-xl font-bold">Data Product</h1>

    @* Search and Controls *@
    <div class="flex flex-col lg:flex-row lg:items-center lg:justify-between my-6 gap-4">
        <div class="flex flex-col lg:flex-row items-stretch lg:items-center gap-4 flex-1">
            @* Search Input *@
            <div class="relative flex-1 lg:min-w-[300px]">
                <div class="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                    <i data-lucide="search" class="w-5 h-5 text-gray-400"></i>
                </div>
                <input type="text" 
                       class="block w-full pl-10 pr-3 py-2 border border-gray-300 rounded-md leading-5 bg-white placeholder-gray-500 focus:outline-none focus:placeholder-gray-400 focus:ring-1 focus:ring-[#FE5F00] focus:border-[#FE5F00]" 
                       placeholder="Search product name">
            </div>

            @* Date Picker Dropdown *@
            <div class="relative">
                <button type="button" class="date-picker-btn w-full lg:w-[200px] inline-flex items-center justify-between px-4 py-2 border border-gray-300 rounded-md bg-white text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-[#FE5F00]">
                    <div class="flex items-center gap-2">
                        <i data-lucide="calendar" class="w-5 h-5 text-gray-400"></i>
                        <span class="date-text">Pick Date</span>
                    </div>
                    <i data-lucide="chevron-down" class="w-5 h-5 text-gray-400"></i>
                </button>
                <div class="date-picker-dropdown absolute left-0 mt-2 w-full bg-white border border-gray-200 rounded-md shadow-lg z-10 hidden">
                    <div class="p-3">
                        <input type="date" class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-[#FE5F00] focus:border-[#FE5F00]">
                    </div>
                </div>
            </div>

            @* Entries Dropdown *@
            <div class="relative">
                <button type="button" class="entries-btn w-full lg:w-[200px] inline-flex items-center justify-between px-4 py-2 border border-gray-300 rounded-md bg-white text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-[#FE5F00]">
                    <span class="entries-text">5 Data Entries</span>
                    <i data-lucide="chevron-down" class="w-5 h-5 text-gray-400"></i>
                </button>
                <div class="entries-dropdown absolute left-0 mt-2 w-full bg-white border border-gray-200 rounded-md shadow-lg z-10 hidden">
                    <div class="py-1">
                        <a href="#" class="entries-option block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100" data-value="5 Data Entries">5 Data Entries</a>
                        <a href="#" class="entries-option block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100" data-value="10 Data Entries">10 Data Entries</a>
                        <a href="#" class="entries-option block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100" data-value="20 Data Entries">20 Data Entries</a>
                        <a href="#" class="entries-option block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100" data-value="50 Data Entries">50 Data Entries</a>
                    </div>
                </div>
            </div>

            @* Add Product Button *@
            <button type="button" class="add-product-btn w-full lg:w-[200px] inline-flex items-center justify-center px-4 py-2 bg-[#FE5F00] text-white rounded-md hover:bg-[#E54900] focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-[#FE5F00]">
                <i data-lucide="plus" class="w-5 h-5 text-white mr-2"></i>
                Add Product
            </button>
        </div>
    </div>

    @* Table *@
    <div class="relative overflow-x-auto sm:rounded-lg">
        <table class="w-full text-sm text-left">
            <thead class="text-md text-gray-700 uppercase border-b border-gray-200">
                <tr>
                    <th scope="col" class="px-6 py-3">
                        IMAGE
                    </th>
                    <th scope="col" class="px-6 py-3">
                        PRODUCT NAME 
                    </th>
                    <th scope="col" class="px-6 py-3">
                        CATEGORY
                    </th>
                    <th scope="col" class="px-6 py-3">
                        PRICE
                    </th>
                    <th scope="col" class="px-6 py-3">
                        MANUFACTURER NAME
                    </th>
                    <th scope="col" class="px-6 py-3">
                        DATE
                    </th>
                    <th scope="col" class="px-6 py-3">
                        ACTION
                    </th>
                </tr>
            </thead>
            <tbody>
                @{
                    var products = new[] {
                        new { Id = 1, Name = "Sandwich", Category = "Food", Price = 25000, Manufacturer = "Muhammad Khairul Kholqi", Date = "12 December 2025" },
                        new { Id = 2, Name = "Burger", Category = "Food", Price = 25000, Manufacturer = "John Doe", Date = "13 December 2025" },
                        new { Id = 3, Name = "Pizza", Category = "Food", Price = 25000, Manufacturer = "Jane Smith", Date = "14 December 2025" },
                        new { Id = 4, Name = "Coffee", Category = "Beverage", Price = 25000, Manufacturer = "Coffee Corp", Date = "15 December 2025" },
                        new { Id = 5, Name = "Juice", Category = "Beverage", Price = 25000, Manufacturer = "Fresh Co", Date = "16 December 2025" }
                    };
                }
                
                @foreach (var product in products)
                {
                    <tr class="text-md hover:bg-gray-100">
                        <td class="px-6 py-4">
                            <img 
                                src="https://encrypted-tbn2.gstatic.com/images?q=tbn:ANd9GcTsmcBGBK53A9NHzfHIsBK0UX5TAhlceYXiPTEntCrtRrf3ZuR5" 
                                alt="@product.Name"
                                class="w-12 h-12 rounded-full object-cover"
                            >
                        </td>
                        <td class="px-6 py-4">
                            @product.Name
                        </td>
                        <td class="px-6 py-4">
                            @product.Category
                        </td>
                        <td class="px-6 py-4">
                            @product.Price
                        </td>
                        <td class="px-6 py-4">
                            @product.Manufacturer
                        </td>
                        <td class="px-6 py-4">
                            @product.Date
                        </td>
                        <td class="px-6 py-4">
                            <div class="flex items-center gap-5">
                                <a 
                                    href="#" 
                                    class="edit-btn text-green-500 hover:underline font-medium"
                                    data-id="@product.Id"
                                    data-name="@product.Name"
                                    data-category="@product.Category"
                                    data-price="@product.Price"
                                    data-manufacturer="@product.Manufacturer"
                                >
                                    Edit
                                </a>
                                <button type="button" class="delete-btn text-red-500 hover:underline font-medium" data-id="@product.Id" data-name="@product.Name">
                                    Delete
                                </button>
                            </div>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
</div>

@* Add Product Modal *@
<div id="addModal" class="modal fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 hidden">
    <div class="modal-content bg-white rounded-lg p-6 w-full max-w-md mx-4">
        <div class="flex justify-between items-center mb-4">
            <h3 class="text-lg font-semibold text-gray-900">Add Product</h3>
            <button type="button" class="close-modal">
                <i data-lucide="x" class="w-6 h-6"></i>
            </button>
        </div>

        <form>
            <div class="mb-6">
                <label class="block text-sm font-medium text-gray-500 mb-2">Image<span class="text-red-500">*</span></label>

                <div id="dropzone"
                    class="relative flex flex-col items-center justify-center w-full h-36 border-2 border-dashed border-gray-300 rounded-md cursor-pointer text-gray-400 hover:border-[#FE5F00] transition-colors"
                    onclick="document.getElementById('fileInput').click()"
                    ondragover="handleDragOver(event)"
                    ondragleave="handleDragLeave(event)"
                    ondrop="handleDrop(event)">

                    <i data-lucide="file-plus" class="h-10 w-10 mb-2 text-gray-400"></i>
                
                    <p id="dropzoneText" class="text-sm text-center px-4">
                        Drag and drop image here or <span class="text-[#FE5F00] font-medium">click to upload</span>
                    </p>
                    
                    <input type="file" id="fileInput" accept="image/*" class="hidden" onchange="handleFileSelect(event)">
                </div>
            </div>

            <div class="mb-4">
                <label class="block text-sm font-medium text-gray-500 mb-2">Product Name<span class="text-red-500">*</span></label>
                <input type="text" class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-[#FE5F00] focus:border-[#FE5F00]" placeholder="Enter product name">
            </div>

            <div class="mb-4">
                <label class="block text-sm font-medium text-gray-500 mb-2">Price<span class="text-red-500">*</span></label>
                <input type="number" class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-[#FE5F00] focus:border-[#FE5F00]" placeholder="Enter product price">
            </div>

            <div class="mb-4">
                <label class="block text-sm font-medium text-gray-500 mb-2">Category<span class="text-red-500">*</span></label>

                <select class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-[#FE5F00] focus:border-[#FE5F00]">
                    <option value="">Select Category</option>
                    <option value="Health">Health</option>
                    <option value="Hobby">Hobby</option>
                    <option value="Food">Food</option>
                    <option value="Drink">Drink</option>
                    <option value="Fashion">Fashion</option>
                    <option value="Electronic">Electronic</option>
                </select>
            </div>

            <button type="submit" class="flex-1 w-full px-4 py-2 bg-[#FE5F00] text-white rounded-md hover:bg-[#E54900] focus:outline-none">
                Add Product
            </button>
        </form>
    </div>
</div>

@* Edit Product Modal *@
<div id="editModal" class="modal fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 hidden">
    <div class="modal-content bg-white rounded-lg p-6 w-full max-w-md mx-4">
        <div class="flex justify-between items-center mb-4">
            <h3 class="text-lg font-semibold text-gray-900">Edit Product</h3>

            <button type="button" class="close-modal text-gray-400 hover:text-gray-600">
                <i data-lucide="x" class="w-6 h-6"></i>
            </button>
        </div>

        <form>
            <input type="hidden" id="editProductId">
            <div class="mb-6 relative group w-full">
                <label class="block text-sm font-medium text-gray-500 mb-2">Image</label>

                <img 
                    id="previewImage"
                    src="https://img.freepik.com/free-psd/macbook-mockup_1332-60596.jpg?t=st=1748253178~exp=1748256778~hmac=81bf0f7ceb822f36424bdf6b835119782cfe207af17a30b00d6edaa95c7b1069&w=740" 
                    alt="Preview"
                    class="w-full h-36 object-cover rounded-md"
                >

                <div class="absolute inset-0 bg-black bg-opacity-40 flex items-center justify-center rounded-md opacity-0 group-hover:opacity-100 transition-opacity">
                    <label class="text-white px-4 py-2 bg-[#FE5F00] rounded cursor-pointer text-sm font-medium">
                        Change Image
                        <input type="file" accept="image/*" class="hidden" onchange="updateImage(event)">
                    </label>
                </div>
            </div>

            <div class="mb-4">
                <label class="block text-sm font-medium text-gray-500 mb-2">Product Name</label>
                <input type="text" id="editProductName" class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-[#FE5F00] focus:border-[#FE5F00]" placeholder="Enter product name">
            </div>

            <div class="mb-4">
                <label class="block text-sm font-medium text-gray-500 mb-2">Price</label>
                <input type="number" id="editPrice" class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-[#FE5F00] focus:border-[#FE5F00]" placeholder="Enter product price">
            </div>

            <div class="mb-4">
                <label class="block text-sm font-medium text-gray-500 mb-2">Category</label>

                <select id="editProductCategory" class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-1 focus:ring-[#FE5F00] focus:border-[#FE5F00]">
                    <option value="">Select Category</option>
                    <option value="Health">Health</option>
                    <option value="Hobby">Hobby</option>
                    <option value="Food">Food</option>
                    <option value="Drink">Drink</option>
                    <option value="Fashion">Fashion</option>
                    <option value="Electronic">Electronic</option>
                </select>
            </div>

            <button type="submit" class="flex-1 w-full px-4 py-2 bg-[#FE5F00] text-white rounded-md hover:bg-[#E54900] focus:outline-none">
                Save Changes
            </button>
        </form>
    </div>
</div>

@* Delete Confirmation Modal *@
<div id="deleteModal" class="modal fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 hidden">
    <div class="modal-content bg-white rounded-lg p-6 w-full max-w-md mx-4">
        <div class="flex items-center justify-center">
            <div class="flex items-center justify-center w-14 h-14 bg-[#FFEDD5] rounded-full">
                <i data-lucide="trash-2" class="w-8 h-8 text-[#FE5F00]"></i>
            </div>
        </div>

        <h1 class="text-center text-xl font-semibold mt-6">Delete Product</h1>

        <p class="text-center text-md font-light text-gray-600 mt-2">Are you sure want to delete this product?</p>

        <p class="text-center text-md font-light text-gray-600">This action cannot be undone</p>

        <div class="flex gap-3 mt-6">
            <button type="button" id="confirmDelete" class="flex-1 px-4 py-2 bg-[#FE5F00] text-white rounded-md hover:bg-[#FE5F00]/90 focus:outline-none">
                Delete
            </button>

            <button type="button" class="close-modal flex-1 px-4 py-2 border border-[#FE5F00] text-[#FE5F00] rounded-md hover:bg-[#FE5F00]/10 focus:outline-none">
                Cancel
            </button>
        </div>
    </div>
</div>

<script>
    document.addEventListener('DOMContentLoaded', function() {
        const tabItems = document.querySelectorAll('.tab-item');
        tabItems.forEach(tab => {
            tab.addEventListener('click', function() {
                tabItems.forEach(t => {
                    t.classList.remove('active');
                    const span = t.querySelector('span');
                    const line = t.querySelector('div');
                    span.classList.remove('text-[#FE5F00]');
                    span.classList.add('text-gray-500');
                    line.classList.remove('bg-[#FE5F00]');
                    line.classList.add('bg-transparent');
                });
                
                this.classList.add('active');
                const span = this.querySelector('span');
                const line = this.querySelector('div');
                span.classList.add('text-[#FE5F00]');
                span.classList.remove('text-gray-500');
                line.classList.add('bg-[#FE5F00]');
                line.classList.remove('bg-transparent');
            });
        });

        const datePickerBtn = document.querySelector('.date-picker-btn');
        const datePickerDropdown = document.querySelector('.date-picker-dropdown');
        const dateInput = datePickerDropdown.querySelector('input[type="date"]');
        const dateText = document.querySelector('.date-text');

        datePickerBtn.addEventListener('click', function(e) {
            e.stopPropagation();
            datePickerDropdown.classList.toggle('hidden');
            document.querySelector('.entries-dropdown').classList.add('hidden');
        });

        dateInput.addEventListener('change', function() {
            if (this.value) {
                const date = new Date(this.value);
                const options = { year: 'numeric', month: 'long', day: 'numeric' };
                dateText.textContent = date.toLocaleDateString('en-US', options);
            } else {
                dateText.textContent = 'Pick Date';
            }
            datePickerDropdown.classList.add('hidden');
        });

        const entriesBtn = document.querySelector('.entries-btn');
        const entriesDropdown = document.querySelector('.entries-dropdown');
        const entriesText = document.querySelector('.entries-text');
        const entriesOptions = document.querySelectorAll('.entries-option');

        entriesBtn.addEventListener('click', function(e) {
            e.stopPropagation();
            entriesDropdown.classList.toggle('hidden');
            datePickerDropdown.classList.add('hidden');
        });

        entriesOptions.forEach(option => {
            option.addEventListener('click', function(e) {
                e.preventDefault();
                entriesText.textContent = this.getAttribute('data-value');
                entriesDropdown.classList.add('hidden');
            });
        });

        document.addEventListener('click', function() {
            datePickerDropdown.classList.add('hidden');
            entriesDropdown.classList.add('hidden');
        });

        const addModal = document.getElementById('addModal');
        const editModal = document.getElementById('editModal');
        const deleteModal = document.getElementById('deleteModal');
        
        const addProductBtn = document.querySelector('.add-product-btn');
        addProductBtn.addEventListener('click', function() {
            addModal.classList.remove('hidden');
        });

        const editBtns = document.querySelectorAll('.edit-btn');
        editBtns.forEach(btn => {
            btn.addEventListener('click', function(e) {
                e.preventDefault();
                const id = this.getAttribute('data-id');
                const name = this.getAttribute('data-name');
                const category = this.getAttribute('data-category');
                const price = this.getAttribute('data-price');
                
                document.getElementById('editProductId').value = id;
                document.getElementById('editProductName').value = name;
                document.getElementById('editProductCategory').value = category;
                document.getElementById('editPrice').value = price;
                
                editModal.classList.remove('hidden');
            });
        });

        const deleteBtns = document.querySelectorAll('.delete-btn');
        let productToDelete = null;
        
        deleteBtns.forEach(btn => {
            btn.addEventListener('click', function() {
                const id = this.getAttribute('data-id');
                const name = this.getAttribute('data-name');
                
                productToDelete = id;
                deleteModal.classList.remove('hidden');
            });
        });

        document.getElementById('confirmDelete').addEventListener('click', function() {
            if (productToDelete) {
                console.log('Deleting product with ID:', productToDelete);
                deleteModal.classList.add('hidden');
                productToDelete = null;
            }
        });

        const closeModalBtns = document.querySelectorAll('.close-modal');
        closeModalBtns.forEach(btn => {
            btn.addEventListener('click', function() {
                addModal.classList.add('hidden');
                editModal.classList.add('hidden');
                deleteModal.classList.add('hidden');
            });
        });

        window.addEventListener('click', function(e) {
            if (e.target === addModal) {
                addModal.classList.add('hidden');
            }
            if (e.target === editModal) {
                editModal.classList.add('hidden');
            }
            if (e.target === deleteModal) {
                deleteModal.classList.add('hidden');
            }
        });
    });
</script>
<script>
    function handleDragOver(event) {
        event.preventDefault();
        document.getElementById('dropzone').classList.add('border-[#FE5F00]');
    }

    function handleDragLeave(event) {
        event.preventDefault();
        document.getElementById('dropzone').classList.remove('border-[#FE5F00]');
    }

    function handleDrop(event) {
        event.preventDefault();
        const fileInput = document.getElementById('fileInput');
        const files = event.dataTransfer.files;
        if (files.length > 0) {
        fileInput.files = files;
        handleFileSelect({ target: { files } });
        }
        document.getElementById('dropzone').classList.remove('border-[#FE5F00]');
    }

    function handleFileSelect(event) {
        const file = event.target.files[0];
        const dropText = document.getElementById('dropzoneText');
        if (file) {
        dropText.textContent = file.name;
        } else {
        dropText.textContent = "Drag and drop image here or click to upload";
        }
    }

    function updateImage(event) {
        const file = event.target.files[0];
        if (file) {
        const reader = new FileReader();
        reader.onload = function(e) {
            document.getElementById('previewImage').src = e.target.result;
        }
        reader.readAsDataURL(file);
        }
    }
</script>