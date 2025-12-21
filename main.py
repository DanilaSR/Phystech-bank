import uuid
import csv
import json
import yaml
from datetime import datetime
from enum import Enum
from typing import List, Optional, Dict, Any
from dataclasses import dataclass, asdict
from decimal import Decimal
from abc import ABC, abstractmethod
import os


# ==================== БАЗОВЫЕ КЛАССЫ ====================

class OperationType(Enum):
    INCOME = "income"
    EXPENSE = "expense"


@dataclass
class BankAccount:
    id: str
    name: str
    balance: Decimal
    
    def __post_init__(self):
        if self.balance < Decimal('0'):
            raise ValueError("Баланс не может быть отрицательным")
    
    def to_dict(self) -> Dict:
        return {
            'id': self.id,
            'name': self.name,
            'balance': str(self.balance)
        }


@dataclass
class Category:
    id: str
    type: OperationType
    name: str
    
    def to_dict(self) -> Dict:
        return {
            'id': self.id,
            'type': self.type.value,
            'name': self.name
        }


@dataclass
class Operation:
    id: str
    type: OperationType
    bank_account_id: str
    amount: Decimal
    date: datetime
    description: Optional[str] = None
    category_id: Optional[str] = None
    
    def __post_init__(self):
        if self.amount <= Decimal('0'):
            raise ValueError("Сумма операции должна быть положительной")
    
    def to_dict(self) -> Dict:
        return {
            'id': self.id,
            'type': self.type.value,
            'bank_account_id': self.bank_account_id,
            'amount': str(self.amount),
            'date': self.date.isoformat(),
            'description': self.description,
            'category_id': self.category_id
        }


# ==================== ИМПОРТ/ЭКСПОРТ ====================

class BaseExporter(ABC):
    @abstractmethod
    def export(self, data: Dict[str, Any], filepath: str) -> None:
        pass


class JSONExporter(BaseExporter):
    def export(self, data: Dict[str, Any], filepath: str) -> None:
        with open(filepath, 'w', encoding='utf-8') as f:
            json.dump(data, f, indent=2, ensure_ascii=False, default=str)


class CSVExporter(BaseExporter):
    def export(self, data: Dict[str, Any], filepath: str) -> None:
        # Экспорт операций в CSV
        operations = data.get('operations', [])
        if operations:
            with open(filepath, 'w', encoding='utf-8', newline='') as f:
                writer = csv.DictWriter(f, fieldnames=operations[0].keys())
                writer.writeheader()
                writer.writerows(operations)
        
        # Экспорт счетов в отдельный файл
        accounts_file = filepath.replace('.csv', '_accounts.csv')
        accounts = data.get('accounts', [])
        if accounts:
            with open(accounts_file, 'w', encoding='utf-8', newline='') as f:
                writer = csv.DictWriter(f, fieldnames=accounts[0].keys())
                writer.writeheader()
                writer.writerows(accounts)
        
        # Экспорт категорий в отдельный файл
        categories_file = filepath.replace('.csv', '_categories.csv')
        categories = data.get('categories', [])
        if categories:
            with open(categories_file, 'w', encoding='utf-8', newline='') as f:
                writer = csv.DictWriter(f, fieldnames=categories[0].keys())
                writer.writeheader()
                writer.writerows(categories)


class YAMLExporter(BaseExporter):
    def export(self, data: Dict[str, Any], filepath: str) -> None:
        with open(filepath, 'w', encoding='utf-8') as f:
            yaml.dump(data, f, allow_unicode=True, default_flow_style=False)


class BaseImporter(ABC):
    @abstractmethod
    def import_data(self, filepath: str) -> Dict[str, Any]:
        pass


class JSONImporter(BaseImporter):
    def import_data(self, filepath: str) -> Dict[str, Any]:
        with open(filepath, 'r', encoding='utf-8') as f:
            return json.load(f)


class CSVImporter(BaseImporter):
    def import_data(self, filepath: str) -> Dict[str, Any]:
        data = {'accounts': [], 'categories': [], 'operations': []}
        
        # Определяем тип файла по имени
        if '_accounts' in filepath:
            with open(filepath, 'r', encoding='utf-8') as f:
                reader = csv.DictReader(f)
                data['accounts'] = list(reader)
        
        elif '_categories' in filepath:
            with open(filepath, 'r', encoding='utf-8') as f:
                reader = csv.DictReader(f)
                data['categories'] = list(reader)
        
        else:
            # Основной файл с операциями
            with open(filepath, 'r', encoding='utf-8') as f:
                reader = csv.DictReader(f)
                data['operations'] = list(reader)
                
                # Пробуем найти связанные файлы
                base_path = filepath.replace('.csv', '')
                accounts_file = base_path + '_accounts.csv'
                categories_file = base_path + '_categories.csv'
                
                if os.path.exists(accounts_file):
                    with open(accounts_file, 'r', encoding='utf-8') as af:
                        account_reader = csv.DictReader(af)
                        data['accounts'] = list(account_reader)
                
                if os.path.exists(categories_file):
                    with open(categories_file, 'r', encoding='utf-8') as cf:
                        category_reader = csv.DictReader(cf)
                        data['categories'] = list(category_reader)
        
        return data


class YAMLImporter(BaseImporter):
    def import_data(self, filepath: str) -> Dict[str, Any]:
        with open(filepath, 'r', encoding='utf-8') as f:
            return yaml.safe_load(f)


class DataManager:
    @staticmethod
    def prepare_export_data(accounts: List[BankAccount], 
                           categories: List[Category], 
                           operations: List[Operation]) -> Dict[str, Any]:
        """Подготовка данных для экспорта"""
        return {
            'accounts': [acc.to_dict() for acc in accounts],
            'categories': [cat.to_dict() for cat in categories],
            'operations': [op.to_dict() for op in operations],
            'export_date': datetime.now().isoformat(),
            'total_accounts': len(accounts),
            'total_operations': len(operations)
        }
    
    @staticmethod
    def create_from_import(data: Dict[str, Any]) -> tuple:
        """Создание объектов из импортированных данных"""
        accounts = []
        categories = []
        operations = []
        
        # Создаем счета
        for acc_data in data.get('accounts', []):
            try:
                account = BankAccount(
                    id=acc_data.get('id', str(uuid.uuid4())),
                    name=acc_data['name'],
                    balance=Decimal(str(acc_data['balance']))
                )
                accounts.append(account)
            except (KeyError, ValueError) as e:
                print(f"Ошибка создания счета: {e}")
        
        # Создаем категории
        for cat_data in data.get('categories', []):
            try:
                category = Category(
                    id=cat_data.get('id', str(uuid.uuid4())),
                    type=OperationType(cat_data['type']),
                    name=cat_data['name']
                )
                categories.append(category)
            except (KeyError, ValueError) as e:
                print(f"Ошибка создания категории: {e}")
        
        # Создаем операции
        for op_data in data.get('operations', []):
            try:
                operation = Operation(
                    id=op_data.get('id', str(uuid.uuid4())),
                    type=OperationType(op_data['type']),
                    bank_account_id=op_data['bank_account_id'],
                    amount=Decimal(str(op_data['amount'])),
                    date=datetime.fromisoformat(op_data['date']),
                    description=op_data.get('description'),
                    category_id=op_data.get('category_id')
                )
                operations.append(operation)
            except (KeyError, ValueError) as e:
                print(f"Ошибка создания операции: {e}")
        
        return accounts, categories, operations


# ==================== ФИНАНСОВЫЙ МЕНЕДЖЕР ====================

class FinanceManager:
    def __init__(self):
        self.accounts: Dict[str, BankAccount] = {}
        self.categories: Dict[str, Category] = {}
        self.operations: Dict[str, Operation] = {}
        
        # Инициализация экспортеров и импортеров
        self.exporters = {
            'json': JSONExporter(),
            'csv': CSVExporter(),
            'yaml': YAMLExporter()
        }
        
        self.importers = {
            'json': JSONImporter(),
            'csv': CSVImporter(),
            'yaml': YAMLImporter()
        }
    
    # === Работа со счетами ===
    def create_account(self, name: str, initial_balance: Decimal = Decimal('0')) -> BankAccount:
        account_id = str(uuid.uuid4())
        account = BankAccount(id=account_id, name=name, balance=initial_balance)
        self.accounts[account_id] = account
        return account
    
    def get_account(self, account_id: str) -> Optional[BankAccount]:
        return self.accounts.get(account_id)
    
    # === Работа с категориями ===
    def create_category(self, type_: OperationType, name: str) -> Category:
        category_id = str(uuid.uuid4())
        category = Category(id=category_id, type=type_, name=name)
        self.categories[category_id] = category
        return category
    
    # === Работа с операциями ===
    def create_operation(self, type_: OperationType, account_id: str,
                        amount: Decimal, date: datetime,
                        description: Optional[str] = None,
                        category_id: Optional[str] = None) -> Operation:
        account = self.get_account(account_id)
        if not account:
            raise ValueError(f"Счет {account_id} не найден")
        
        if category_id and category_id not in self.categories:
            raise ValueError(f"Категория {category_id} не найдена")
        
        operation_id = str(uuid.uuid4())
        operation = Operation(
            id=operation_id,
            type=type_,
            bank_account_id=account_id,
            amount=amount,
            date=date,
            description=description,
            category_id=category_id
        )
        
        # Обновляем баланс
        if type_ == OperationType.INCOME:
            account.balance += amount
        else:
            if account.balance < amount:
                raise ValueError("Недостаточно средств")
            account.balance -= amount
        
        self.operations[operation_id] = operation
        return operation
    
    # === Экспорт данных ===
    def export_data(self, filepath: str, format: str = 'json') -> None:
        """Экспорт всех данных в файл"""
        if format not in self.exporters:
            raise ValueError(f"Не поддерживается формат: {format}")
        
        # Подготавливаем данные
        data = DataManager.prepare_export_data(
            list(self.accounts.values()),
            list(self.categories.values()),
            list(self.operations.values())
        )
        
        # Определяем расширение файла
        if not filepath.endswith(f'.{format}'):
            filepath = f"{filepath}.{format}"
        
        # Экспортируем
        exporter = self.exporters[format]
        exporter.export(data, filepath)
        
        print(f"Данные экспортированы в {filepath}")
        print(f"Счетов: {len(self.accounts)}, "
              f"Категорий: {len(self.categories)}, "
              f"Операций: {len(self.operations)}")
    
    # === Импорт данных ===
    def import_data(self, filepath: str, format: str = None) -> None:
        """Импорт данных из файла"""
        # Определяем формат по расширению файла
        if format is None:
            ext = filepath.split('.')[-1].lower()
            if ext in self.importers:
                format = ext
            else:
                raise ValueError(f"Неизвестный формат файла: {filepath}")
        
        if format not in self.importers:
            raise ValueError(f"Не поддерживается формат: {format}")
        
        # Импортируем данные
        importer = self.importers[format]
        data = importer.import_data(filepath)
        
        # Создаем объекты
        accounts, categories, operations = DataManager.create_from_import(data)
        
        # Добавляем в менеджер
        for account in accounts:
            self.accounts[account.id] = account
        
        for category in categories:
            self.categories[category.id] = category
        
        for operation in operations:
            self.operations[operation.id] = operation
        
        print(f"Данные импортированы из {filepath}")
        print(f"Загружено: {len(accounts)} счетов, "
              f"{len(categories)} категорий, "
              f"{len(operations)} операций")
    
    # === Аналитика ===
    def get_balance_difference(self, start_date: datetime, end_date: datetime) -> Decimal:
        total_income = Decimal('0')
        total_expense = Decimal('0')
        
        for op in self.operations.values():
            if start_date <= op.date <= end_date:
                if op.type == OperationType.INCOME:
                    total_income += op.amount
                else:
                    total_expense += op.amount
        
        return total_income - total_expense
    
    def group_by_category(self, start_date: datetime, end_date: datetime) -> Dict[str, Decimal]:
        result = {}
        
        for op in self.operations.values():
            if start_date <= op.date <= end_date:
                if op.category_id:
                    category = self.categories.get(op.category_id)
                    category_name = category.name if category else "Без категории"
                else:
                    category_name = "Без категории"
                
                if category_name not in result:
                    result[category_name] = Decimal('0')
                
                if op.type == OperationType.INCOME:
                    result[category_name] += op.amount
                else:
                    result[category_name] -= op.amount
        
        return result
    
    # === Пересчет балансов ===
    def recalculate_balances(self) -> None:
        """Пересчет балансов всех счетов"""
        # Сбрасываем балансы
        for account in self.accounts.values():
            account.balance = Decimal('0')
        
        # Пересчитываем операции
        for op in self.operations.values():
            account = self.accounts.get(op.bank_account_id)
            if account:
                if op.type == OperationType.INCOME:
                    account.balance += op.amount
                else:
                    account.balance -= op.amount
        
        print("Балансы всех счетов пересчитаны")


# ==================== ПРИМЕР ИСПОЛЬЗОВАНИЯ ====================

def main():
    print("=== СИСТЕМА УЧЕТА ФИНАНСОВ С ИМПОРТОМ/ЭКСПОРТОМ ===\n")
    
    # Создаем менеджер
    manager = FinanceManager()
    
    # 1. Создаем тестовые данные
    print("1. СОЗДАНИЕ ТЕСТОВЫХ ДАННЫХ:")
    
    # Счета
    main_account = manager.create_account("Основной счет", Decimal('10000'))
    savings_account = manager.create_account("Накопительный", Decimal('5000'))
    print(f"   Создано счетов: {len(manager.accounts)}")
    
    # Категории
    salary_cat = manager.create_category(OperationType.INCOME, "Зарплата")
    food_cat = manager.create_category(OperationType.EXPENSE, "Еда")
    transport_cat = manager.create_category(OperationType.EXPENSE, "Транспорт")
    print(f"   Создано категорий: {len(manager.categories)}")
    
    # Операции
    manager.create_operation(
        OperationType.INCOME, main_account.id, Decimal('50000'),
        datetime.now(), "Зарплата", salary_cat.id
    )
    manager.create_operation(
        OperationType.EXPENSE, main_account.id, Decimal('3000'),
        datetime.now(), "Продукты", food_cat.id
    )
    manager.create_operation(
        OperationType.EXPENSE, main_account.id, Decimal('1500'),
        datetime.now(), "Такси", transport_cat.id
    )
    print(f"   Создано операций: {len(manager.operations)}")
    
    # 2. Экспорт данных в разные форматы
    print("\n2. ЭКСПОРТ ДАННЫХ:")
    
    # JSON
    manager.export_data("финансы", "json")
    
    # CSV (создаст 3 файла)
    manager.export_data("финансы", "csv")
    
    # YAML
    manager.export_data("финансы", "yaml")
    
    print("   Файлы созданы: финансы.json, финансы.csv, финансы.yaml")
    
    # 3. Создаем новый менеджер для импорта
    print("\n3. ИМПОРТ ДАННЫХ ИЗ JSON:")
    new_manager = FinanceManager()
    
    # Импортируем данные из JSON
    new_manager.import_data("финансы.json")
    
    # Проверяем импортированные данные
    print(f"   Импортировано счетов: {len(new_manager.accounts)}")
    print(f"   Импортировано категорий: {len(new_manager.categories)}")
    print(f"   Импортировано операций: {len(new_manager.operations)}")
    
    # 4. Проверяем балансы
    print("\n4. ПРОВЕРКА БАЛАНСОВ:")
    for account in new_manager.accounts.values():
        print(f"   {account.name}: {account.balance} руб.")
    
    # 5. Аналитика
    print("\n5. АНАЛИТИКА:")
    start_date = datetime(2024, 1, 1)
    end_date = datetime(2024, 12, 31)
    
    balance_diff = new_manager.get_balance_difference(start_date, end_date)
    print(f"   Разница доходов и расходов: {balance_diff} руб.")
    
    # 6. Пересчет балансов
    print("\n6. ПЕРЕСЧЕТ БАЛАНСОВ:")
    new_manager.recalculate_balances()
    
    print("\n=== РАБОТА ЗАВЕРШЕНА ===")


def quick_examples():
    """Быстрые примеры использования"""
    manager = FinanceManager()
    
    # Пример 1: Экспорт в JSON
    manager.create_account("Тестовый счет", Decimal('1000'))
    manager.export_data("test", "json")
    
    # Пример 2: Импорт из CSV
    try:
        manager.import_data("test.csv", "csv")
    except FileNotFoundError:
        print("Файл test.csv не найден")
    
    # Пример 3: Экспорт всех форматов
    formats = ['json', 'csv', 'yaml']
    for fmt in formats:
        try:
            manager.export_data(f"export_{fmt}", fmt)
        except Exception as e:
            print(f"Ошибка экспорта в {fmt}: {e}")


if __name__ == "__main__":
    main()
    quick_examples()  # Раскомментируйте для быстрых примеров