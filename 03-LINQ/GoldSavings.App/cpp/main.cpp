#include <iostream>
#include <vector>
#include <string>
#include <stdexcept>
#include <cstdlib>
#include <ctime>

template<typename T>
class RandomizedList {
private:
    std::vector<T> items;

public:
    RandomizedList() {}

    void add(const T& element) {
        bool addAtEnd = (rand() % 2) == 0;

        if (addAtEnd) {
            items.push_back(element);
            std::cout << "[Add] added at the END\n";
        } else {
            items.insert(items.begin(), element);
            std::cout << "[Add] added at the START\n";
        }

        printAll();
    }

    T get(int index) {
        if (isEmpty()) {
            throw std::runtime_error("Collection is empty.");
        }

        int maxIndex = std::min(index, (int)items.size() - 1);
        int randomIndex = rand() % (maxIndex + 1);

        std::cout << "[Get] Requested: " << index
                  << " | Used: " << randomIndex << "\n";

        return items[randomIndex];
    }

    bool isEmpty() const {
        return items.empty();
    }

    void printAll() const {
        std::cout << "[List] Count: " << items.size() << " | Items: [";
        for (size_t i = 0; i < items.size(); ++i) {
            std::cout << items[i];
            if (i + 1 < items.size()) std::cout << ", ";
        }
        std::cout << "]\n";
    }
};

int main() {
    srand(static_cast<unsigned>(time(nullptr)));

    std::cout << std::string(60, '=') << "\n";
    std::cout << "RandomizedList<T> in C++ Templates\n";
    std::cout << std::string(60, '=') << "\n";

    // --- Integer test ---
    std::cout << "\n[INTEGER LIST TEST]\n";
    RandomizedList<int> intList;

    std::cout << "IsEmpty: " << (intList.isEmpty() ? "true" : "false") << "\n\n";

    intList.add(10);
    intList.add(20);
    intList.add(30);
    intList.add(40);
    intList.add(50);

    std::cout << "\nIsEmpty: " << (intList.isEmpty() ? "true" : "false") << "\n";

    std::cout << "\nGet(3) calls:\n";
    for (int i = 0; i < 5; ++i) {
        int value = intList.get(3);
        std::cout << "  -> Got value: " << value << "\n";
    }

    // --- String test ---
    std::cout << "\n[STRING LIST TEST]\n";
    RandomizedList<std::string> stringList;

    std::cout << "IsEmpty: " << (stringList.isEmpty() ? "true" : "false") << "\n\n";

    stringList.add("Alice");
    stringList.add("Bob");
    stringList.add("Charlie");
    stringList.add("Diana");
    stringList.add("Eve");

    std::cout << "\nGet(2) calls:\n";
    for (int i = 0; i < 5; ++i) {
        std::string value = stringList.get(2);
        std::cout << "  -> Got value: " << value << "\n";
    }

    std::cout << std::string(60, '=') << "\n";
    return 0;
}
